using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Services;

public class SoporteService : ISoporteService
{
    private readonly ApplicationDbContext _context;

    public SoporteService(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Métodos para Reclamos

    public async Task<List<Reclamo>> GetReclamosAsync()
    {
        return await _context.Reclamos
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Reclamo>> GetReclamosPorEstadoAsync(EstadoReclamo estado)
    {
        return await _context.Reclamos
            .Where(r => r.Estado == estado)
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Reclamo>> GetReclamosPorFiltroAsync(string? tipoProblema = null, EstadoReclamo? estado = null, string? prioridad = null)
    {
        var query = _context.Reclamos
            .AsQueryable();

        if (!string.IsNullOrEmpty(tipoProblema))
            query = query.Where(r => r.TipoProblema == tipoProblema);

        if (estado.HasValue)
            query = query.Where(r => r.Estado == estado.Value);

        if (!string.IsNullOrEmpty(prioridad))
            query = query.Where(r => r.Prioridad == prioridad);

        return await query
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Reclamo?> GetReclamoByIdAsync(int id)
    {
        return await _context.Reclamos
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<int> CrearReclamoAsync(Reclamo reclamo)
    {
        try
        {
            reclamo.FechaCreacion = DateTime.UtcNow;
            reclamo.Estado = EstadoReclamo.Pendiente;

            _context.Reclamos.Add(reclamo);
            await _context.SaveChangesAsync();
            return reclamo.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando reclamo: {ex.Message}");
            return 0;
        }
    }

    public async Task<bool> ActualizarReclamoAsync(Reclamo reclamo)
    {
        try
        {
            _context.Reclamos.Update(reclamo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error actualizando reclamo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ResponderReclamoAsync(int reclamoId, string respuesta, string emailAdmin)
    {
        try
        {
            var reclamo = await _context.Reclamos.FindAsync(reclamoId);
            if (reclamo == null) return false;

            reclamo.RespuestaAdmin = respuesta;
            reclamo.EmailAdminRespuesta = emailAdmin;
            reclamo.FechaRespuesta = DateTime.UtcNow;
            reclamo.Estado = EstadoReclamo.Resuelto;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error respondiendo reclamo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CambiarEstadoReclamoAsync(int reclamoId, EstadoReclamo nuevoEstado)
    {
        try
        {
            var reclamo = await _context.Reclamos.FindAsync(reclamoId);
            if (reclamo == null) return false;

            reclamo.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cambiando estado del reclamo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CambiarPrioridadReclamoAsync(int reclamoId, string nuevaPrioridad)
    {
        try
        {
            var reclamo = await _context.Reclamos.FindAsync(reclamoId);
            if (reclamo == null) return false;

            reclamo.Prioridad = nuevaPrioridad;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cambiando prioridad del reclamo: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Métodos para Mensajes

    public async Task<List<Mensaje>> GetMensajesAsync()
    {
        return await _context.Mensajes
            .OrderByDescending(m => m.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Mensaje>> GetMensajesPorEstadoAsync(EstadoMensaje estado)
    {
        return await _context.Mensajes
            .Where(m => m.Estado == estado)
            .OrderByDescending(m => m.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Mensaje>> GetMensajesPorFiltroAsync(string? tipoConsulta = null, EstadoMensaje? estado = null, string? prioridad = null)
    {
        var query = _context.Mensajes
            .AsQueryable();

        if (!string.IsNullOrEmpty(tipoConsulta))
            query = query.Where(m => m.TipoConsulta == tipoConsulta);

        if (estado.HasValue)
            query = query.Where(m => m.Estado == estado.Value);

        if (!string.IsNullOrEmpty(prioridad))
            query = query.Where(m => m.Prioridad == prioridad);

        return await query
            .OrderByDescending(m => m.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Mensaje?> GetMensajeByIdAsync(int id)
    {
        return await _context.Mensajes
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<int> CrearMensajeAsync(Mensaje mensaje)
    {
        try
        {
            // Validar datos antes de guardar
            if (string.IsNullOrEmpty(mensaje.EmailCliente))
                throw new ArgumentException("El email del cliente es requerido");
            if (string.IsNullOrEmpty(mensaje.Nombre))
                throw new ArgumentException("El nombre es requerido");
            if (string.IsNullOrEmpty(mensaje.Apellidos))
                throw new ArgumentException("Los apellidos son requeridos");
            if (string.IsNullOrEmpty(mensaje.TipoConsulta))
                throw new ArgumentException("El tipo de consulta es requerido");
            if (string.IsNullOrEmpty(mensaje.ContenidoMensaje))
                throw new ArgumentException("El contenido del mensaje es requerido");
            if (string.IsNullOrEmpty(mensaje.Asunto))
                throw new ArgumentException("El asunto es requerido");

            mensaje.FechaCreacion = DateTime.UtcNow;
            mensaje.Estado = EstadoMensaje.NoLeido;

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();
            return mensaje.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando mensaje: {ex}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            return 0;
        }
    }

    public async Task<bool> ActualizarMensajeAsync(Mensaje mensaje)
    {
        try
        {
            _context.Mensajes.Update(mensaje);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error actualizando mensaje: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ResponderMensajeAsync(int mensajeId, string respuesta, string emailAdmin)
    {
        try
        {
            var mensaje = await _context.Mensajes.FindAsync(mensajeId);
            if (mensaje == null) return false;

            mensaje.RespuestaAdmin = respuesta;
            mensaje.EmailAdminRespuesta = emailAdmin;
            mensaje.FechaRespuesta = DateTime.UtcNow;
            mensaje.Estado = EstadoMensaje.Respondido;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error respondiendo mensaje: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CambiarEstadoMensajeAsync(int mensajeId, EstadoMensaje nuevoEstado)
    {
        try
        {
            var mensaje = await _context.Mensajes.FindAsync(mensajeId);
            if (mensaje == null) return false;

            mensaje.Estado = nuevoEstado;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cambiando estado del mensaje: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CambiarPrioridadMensajeAsync(int mensajeId, string nuevaPrioridad)
    {
        try
        {
            var mensaje = await _context.Mensajes.FindAsync(mensajeId);
            if (mensaje == null) return false;

            mensaje.Prioridad = nuevaPrioridad;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cambiando prioridad del mensaje: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Métodos para estadísticas

    public async Task<int> GetReclamosPendientesCountAsync()
    {
        return await _context.Reclamos
            .CountAsync(r => r.Estado == EstadoReclamo.Pendiente);
    }

    public async Task<int> GetMensajesNoLeidosCountAsync()
    {
        return await _context.Mensajes
            .CountAsync(m => m.Estado == EstadoMensaje.NoLeido);
    }

    public async Task<Dictionary<string, int>> GetEstadisticasReclamosAsync()
    {
        return new Dictionary<string, int>
        {
            ["Pendientes"] = await _context.Reclamos.CountAsync(r => r.Estado == EstadoReclamo.Pendiente),
            ["EnProceso"] = await _context.Reclamos.CountAsync(r => r.Estado == EstadoReclamo.EnProceso),
            ["Resueltos"] = await _context.Reclamos.CountAsync(r => r.Estado == EstadoReclamo.Resuelto),
            ["Cerrados"] = await _context.Reclamos.CountAsync(r => r.Estado == EstadoReclamo.Cerrado),
            ["Total"] = await _context.Reclamos.CountAsync()
        };
    }

    public async Task<Dictionary<string, int>> GetEstadisticasMensajesAsync()
    {
        return new Dictionary<string, int>
        {
            ["NoLeidos"] = await _context.Mensajes.CountAsync(m => m.Estado == EstadoMensaje.NoLeido),
            ["Leidos"] = await _context.Mensajes.CountAsync(m => m.Estado == EstadoMensaje.Leido),
            ["Respondidos"] = await _context.Mensajes.CountAsync(m => m.Estado == EstadoMensaje.Respondido),
            ["Archivados"] = await _context.Mensajes.CountAsync(m => m.Estado == EstadoMensaje.Archivado),
            ["Total"] = await _context.Mensajes.CountAsync()
        };
    }

    #endregion

    #region Métodos para opciones de formularios

    public async Task<List<string>> GetTiposProblemaAsync()
    {
        // Obtener tipos únicos de problemas de la base de datos
        var tiposFromDb = await _context.Reclamos
            .Select(r => r.TipoProblema)
            .Distinct()
            .ToListAsync();

        // Combinar con tipos predefinidos
        var tiposPredefinidos = new List<string>
        {
            "Problemas Técnicos",
            "Problemas con Anuncio", 
            "Problemas con Pago",
            "Problemas con Cuenta",
            "Reportar Usuario",
            "Solicitud Eliminación Cuenta",
            "Otro"
        };

        var todosLosTipos = tiposPredefinidos.Union(tiposFromDb).Distinct().ToList();
        return todosLosTipos;
    }

    public async Task<List<string>> GetTiposConsultaAsync()
    {
        // Obtener tipos únicos de consultas de la base de datos
        var tiposFromDb = await _context.Mensajes
            .Select(m => m.TipoConsulta)
            .Distinct()
            .ToListAsync();

        // Combinar con tipos predefinidos
        var tiposPredefinidos = new List<string>
        {
            "Información General",
            "Consulta Comercial",
            "Soporte Técnico",
            "Sugerencias",
            "Colaboraciones",
            "Publicidad",
            "Otro"
        };

        var todosLosTipos = tiposPredefinidos.Union(tiposFromDb).Distinct().ToList();
        return todosLosTipos;
    }

    public List<string> GetPrioridades()
    {
        return new List<string> { "Baja", "Media", "Alta", "Crítica" };
    }

    #endregion
}