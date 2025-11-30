using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoClick.Pages
{
    public class PerfilAgenciaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PerfilAgenciaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string AgenciaEmail { get; set; } = string.Empty;

        public Usuario? Agencia { get; set; }
        public List<Auto> AutosAgencia { get; set; } = new List<Auto>();
        public int TotalAutos { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;

        // Filtros
        [BindProperty(SupportsGet = true)]
        public string? Province { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Brand { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Model { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinKm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxKm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MaxYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? BodyType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FuelType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Transmission { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Condition { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        // Listas para los filtros
        public List<string> AvailableProvinces { get; set; } = new();
        public List<string> AvailableCantons { get; set; } = new();
        public List<string> AvailableBrands { get; set; } = new();
        public List<string> AvailableModels { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(AgenciaEmail))
            {
                return NotFound();
            }

            // Cargar la información de la agencia
            Agencia = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == AgenciaEmail && !string.IsNullOrEmpty(u.NombreAgencia));

            if (Agencia == null)
            {
                return NotFound();
            }

            CurrentPage = PageNumber > 0 ? PageNumber : 1;

            // Consulta base: autos activos de esta agencia
            var query = _context.Autos
                .Include(a => a.Propietario)
                .Where(a => a.EmailPropietario == AgenciaEmail && a.Activo && a.PlanVisibilidad > 0);

            // Aplicar filtros
            if (!string.IsNullOrEmpty(Province))
            {
                query = query.Where(a => a.Provincia == Province);
            }

            if (!string.IsNullOrEmpty(Canton))
            {
                query = query.Where(a => a.Canton == Canton);
            }

            if (!string.IsNullOrEmpty(Brand))
            {
                query = query.Where(a => a.Marca == Brand);
            }

            if (!string.IsNullOrEmpty(Model))
            {
                query = query.Where(a => a.Modelo == Model);
            }

            if (MinPrice.HasValue)
            {
                query = query.Where(a => a.Precio >= MinPrice.Value);
            }

            if (MaxPrice.HasValue)
            {
                query = query.Where(a => a.Precio <= MaxPrice.Value);
            }

            if (MinKm.HasValue)
            {
                query = query.Where(a => a.Kilometraje >= MinKm.Value);
            }

            if (MaxKm.HasValue)
            {
                query = query.Where(a => a.Kilometraje <= MaxKm.Value);
            }

            if (MinYear.HasValue)
            {
                query = query.Where(a => a.Ano >= MinYear.Value);
            }

            if (MaxYear.HasValue)
            {
                query = query.Where(a => a.Ano <= MaxYear.Value);
            }

            if (!string.IsNullOrEmpty(BodyType))
            {
                query = query.Where(a => a.Carroceria == BodyType);
            }

            if (!string.IsNullOrEmpty(FuelType))
            {
                query = query.Where(a => a.Combustible == FuelType);
            }

            if (!string.IsNullOrEmpty(Transmission))
            {
                query = query.Where(a => a.Transmision == Transmission);
            }

            if (!string.IsNullOrEmpty(Condition))
            {
                query = query.Where(a => a.Condicion == Condition);
            }

            // Ordenar
            query = SortBy switch
            {
                "price-asc" => query.OrderBy(a => a.Precio),
                "price-desc" => query.OrderByDescending(a => a.Precio),
                "year-desc" => query.OrderByDescending(a => a.Ano),
                "year-asc" => query.OrderBy(a => a.Ano),
                "km-asc" => query.OrderBy(a => a.Kilometraje),
                "km-desc" => query.OrderByDescending(a => a.Kilometraje),
                _ => query.OrderByDescending(a => a.FechaCreacion), // Más recientes por defecto
            };

            // Contar total
            TotalAutos = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(TotalAutos / (double)PageSize);

            // Paginación
            AutosAgencia = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Cargar opciones para filtros
            await LoadFilterOptions();

            return Page();
        }

        private async Task LoadFilterOptions()
        {
            var autosAgencia = _context.Autos
                .Where(a => a.EmailPropietario == AgenciaEmail && a.Activo && a.PlanVisibilidad > 0);

            AvailableProvinces = await autosAgencia
                .Where(a => !string.IsNullOrEmpty(a.Provincia))
                .Select(a => a.Provincia!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            AvailableCantons = await autosAgencia
                .Where(a => !string.IsNullOrEmpty(a.Canton))
                .Select(a => a.Canton!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            AvailableBrands = await autosAgencia
                .Where(a => !string.IsNullOrEmpty(a.Marca))
                .Select(a => a.Marca!)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            AvailableModels = await autosAgencia
                .Where(a => !string.IsNullOrEmpty(a.Modelo))
                .Select(a => a.Modelo!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
    }
}
