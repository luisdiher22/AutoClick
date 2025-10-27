using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Services;

public interface IAutoService
{
    Task<List<Auto>> GetAutosDestacadosAsync(int cantidad = 3);
    Task<List<Auto>> GetAutosRecientesAsync(int cantidad = 3);
    Task<List<Auto>> GetAutosAleatoriosAsync(int cantidad = 3);
    Task<List<Auto>> GetAutosPorFiltrosAsync(AutoFiltros filtros);
    Task<Auto?> GetAutoByIdAsync(int id);
    Task<PaginatedResult<Auto>> GetAutosPaginadosAsync(int page, int pageSize, string sortBy = "recent", AutoFiltros? filtros = null);
}

public class AutoService : IAutoService
{
    private readonly ApplicationDbContext _context;

    public AutoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Auto>> GetAutosDestacadosAsync(int cantidad = 3)
    {
        return await _context.Autos
            .Where(a => a.Activo && a.PlanVisibilidad > 1)
            .OrderByDescending(a => a.PlanVisibilidad)
            .ThenByDescending(a => a.FechaCreacion)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<List<Auto>> GetAutosRecientesAsync(int cantidad = 3)
    {
        return await _context.Autos
            .Where(a => a.Activo && a.PlanVisibilidad > 0) // Excluir anuncios pendientes de aprobación
            .OrderByDescending(a => a.FechaCreacion)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<List<Auto>> GetAutosAleatoriosAsync(int cantidad = 3)
    {
        // SQLite doesn't support Guid.NewGuid() in queries, so we'll use a different approach
        var totalAutos = await _context.Autos.CountAsync(a => a.Activo && a.PlanVisibilidad > 0); // Excluir pendientes
        if (totalAutos == 0) return new List<Auto>();
        
        var random = new Random();
        var skipCount = random.Next(0, Math.Max(1, totalAutos - cantidad));
        
        return await _context.Autos
            .Where(a => a.Activo && a.PlanVisibilidad > 0) // Excluir anuncios pendientes de aprobación
            .Skip(skipCount)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<List<Auto>> GetAutosPorFiltrosAsync(AutoFiltros filtros)
    {
        var query = _context.Autos.Where(a => a.Activo && a.PlanVisibilidad > 0); // Excluir anuncios pendientes

        if (!string.IsNullOrEmpty(filtros.Marca))
            query = query.Where(a => a.Marca.ToLower().Contains(filtros.Marca.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Modelo))
            query = query.Where(a => a.Modelo.ToLower().Contains(filtros.Modelo.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Provincia))
            query = query.Where(a => a.Provincia.ToLower().Contains(filtros.Provincia.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Canton))
            query = query.Where(a => a.Canton.ToLower().Contains(filtros.Canton.ToLower()));

        if (filtros.PrecioMin.HasValue)
            query = query.Where(a => a.Precio >= filtros.PrecioMin.Value);

        if (filtros.PrecioMax.HasValue)
            query = query.Where(a => a.Precio <= filtros.PrecioMax.Value);

        if (filtros.AnoMin.HasValue)
            query = query.Where(a => a.Ano >= filtros.AnoMin.Value);

        if (filtros.AnoMax.HasValue)
            query = query.Where(a => a.Ano <= filtros.AnoMax.Value);

        if (filtros.KilometrajeMin.HasValue)
            query = query.Where(a => a.Kilometraje >= filtros.KilometrajeMin.Value);

        if (filtros.KilometrajeMax.HasValue)
            query = query.Where(a => a.Kilometraje <= filtros.KilometrajeMax.Value);

        if (!string.IsNullOrEmpty(filtros.Carroceria))
            query = query.Where(a => a.Carroceria.ToLower().Contains(filtros.Carroceria.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Combustible))
            query = query.Where(a => a.Combustible.ToLower().Contains(filtros.Combustible.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Transmision))
            query = query.Where(a => a.Transmision.ToLower().Contains(filtros.Transmision.ToLower()));

        if (!string.IsNullOrEmpty(filtros.Condicion))
            query = query.Where(a => a.Condicion.ToLower().Contains(filtros.Condicion.ToLower()));

        if (filtros.SoloDestacados)
            query = query.Where(a => a.PlanVisibilidad > 1);

        return await query.ToListAsync();
    }

    public async Task<Auto?> GetAutoByIdAsync(int id)
    {
        return await _context.Autos
            .Include(a => a.Propietario)
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo && a.PlanVisibilidad > 0); // Excluir pendientes
    }

    public async Task<PaginatedResult<Auto>> GetAutosPaginadosAsync(int page, int pageSize, string sortBy = "recent", AutoFiltros? filtros = null)
    {
        var query = _context.Autos.Where(a => a.Activo && a.PlanVisibilidad > 0); // Excluir anuncios pendientes

        // Apply filters if provided
        if (filtros != null)
        {
            if (!string.IsNullOrEmpty(filtros.Marca))
                query = query.Where(a => a.Marca.ToLower().Contains(filtros.Marca.ToLower()));

            if (!string.IsNullOrEmpty(filtros.Modelo))
                query = query.Where(a => a.Modelo.ToLower().Contains(filtros.Modelo.ToLower()));

            if (filtros.PrecioMin.HasValue)
                query = query.Where(a => a.Precio >= filtros.PrecioMin.Value);

            if (filtros.PrecioMax.HasValue)
                query = query.Where(a => a.Precio <= filtros.PrecioMax.Value);

            if (filtros.AnoMin.HasValue)
                query = query.Where(a => a.Ano >= filtros.AnoMin.Value);

            if (filtros.AnoMax.HasValue)
                query = query.Where(a => a.Ano <= filtros.AnoMax.Value);

            if (filtros.SoloDestacados)
                query = query.Where(a => a.PlanVisibilidad > 1);
        }

        // Apply sorting - Convert decimal to double for SQLite compatibility
        query = sortBy switch
        {
            "price-asc" => query.OrderBy(a => (double)a.Precio),
            "price-desc" => query.OrderByDescending(a => (double)a.Precio),
            "year" => query.OrderByDescending(a => a.Ano),
            "brand" => query.OrderBy(a => a.Marca).ThenBy(a => a.Modelo),
            "featured" => query.OrderByDescending(a => a.PlanVisibilidad).ThenByDescending(a => a.FechaCreacion),
            _ => query.OrderByDescending(a => a.FechaCreacion) // "recent"
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Auto>
        {
            Items = items,
            CurrentPage = page,
            TotalPages = totalPages,
            TotalCount = totalCount,
            PageSize = pageSize
        };
    }
}

