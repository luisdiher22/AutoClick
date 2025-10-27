using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class AdminApprovalController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminApprovalController> _logger;

    public AdminApprovalController(ApplicationDbContext context, ILogger<AdminApprovalController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> ApproveAd(int id)
    {
        try
        {
            // Usar Include para asegurarnos de que no hay problemas de tracking
            var auto = await _context.Autos
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
            
            if (auto == null)
            {
                _logger.LogWarning($"Intento de aprobar anuncio {id} que no existe");
                return NotFound(new { success = false, message = "Anuncio no encontrado" });
            }

            _logger.LogInformation($"Auto encontrado - ID: {id}, PlanVisibilidad actual: {auto.PlanVisibilidad}, Activo: {auto.Activo}");

            if (auto.PlanVisibilidad != 0)
            {
                _logger.LogWarning($"Intento de aprobar anuncio {id} que no está pendiente (PlanVisibilidad = {auto.PlanVisibilidad})");
                return BadRequest(new { success = false, message = "Este anuncio no está pendiente de aprobación" });
            }

            // Aprobar el anuncio cambiando PlanVisibilidad de 0 a 1
            auto.PlanVisibilidad = 1;
            auto.FechaActualizacion = DateTime.UtcNow;
            
            _logger.LogInformation($"Aprobando anuncio {id} - Cambiando PlanVisibilidad de 0 a 1");

            // Marcar explícitamente la entidad como modificada
            _context.Entry(auto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            
            var changes = await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Cambios guardados: {changes} registro(s) afectado(s)");

            // Recargar la entidad desde la base de datos para confirmar
            await _context.Entry(auto).ReloadAsync();
            
            _logger.LogInformation($"Anuncio {id} ({auto.NombreCompleto}) aprobado exitosamente. PlanVisibilidad = {auto.PlanVisibilidad}, Activo = {auto.Activo}");

            return Ok(new 
            { 
                success = true, 
                message = $"El anuncio '{auto.NombreCompleto}' fue aprobado exitosamente",
                title = auto.NombreCompleto,
                debug = new 
                {
                    id = auto.Id,
                    planVisibilidad = auto.PlanVisibilidad,
                    activo = auto.Activo,
                    changesSaved = changes
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al aprobar anuncio {id}");
            return StatusCode(500, new { success = false, message = "Error al aprobar el anuncio", error = ex.Message });
        }
    }

    [HttpPost("reject/{id}")]
    public async Task<IActionResult> RejectAd(int id)
    {
        try
        {
            var auto = await _context.Autos.FindAsync(id);
            
            if (auto == null)
            {
                return NotFound(new { success = false, message = "Anuncio no encontrado" });
            }

            if (auto.PlanVisibilidad != 0)
            {
                return BadRequest(new { success = false, message = "Este anuncio no está pendiente de aprobación" });
            }

            // Marcar como inactivo en lugar de eliminar
            auto.Activo = false;
            auto.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Anuncio {id} ({auto.NombreCompleto}) rechazado por el administrador");

            return Ok(new 
            { 
                success = true, 
                message = $"El anuncio '{auto.NombreCompleto}' fue rechazado",
                title = auto.NombreCompleto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al rechazar anuncio {id}");
            return StatusCode(500, new { success = false, message = "Error al rechazar el anuncio" });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingAds()
    {
        try
        {
            var pendingAds = await _context.Autos
                .Where(a => a.PlanVisibilidad == 0 && a.Activo)
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new
                {
                    id = a.Id,
                    title = a.NombreCompleto,
                    date = a.FechaCreacion
                })
                .ToListAsync();

            return Ok(new { success = true, data = pendingAds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener anuncios pendientes");
            return StatusCode(500, new { success = false, message = "Error al obtener anuncios pendientes" });
        }
    }

    [HttpGet("check/{id}")]
    public async Task<IActionResult> CheckAdStatus(int id)
    {
        try
        {
            var auto = await _context.Autos.FindAsync(id);
            
            if (auto == null)
            {
                return NotFound(new { success = false, message = "Anuncio no encontrado" });
            }

            return Ok(new 
            { 
                success = true, 
                id = auto.Id,
                title = auto.NombreCompleto,
                planVisibilidad = auto.PlanVisibilidad,
                activo = auto.Activo,
                fechaCreacion = auto.FechaCreacion,
                fechaActualizacion = auto.FechaActualizacion
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al verificar estado del anuncio {id}");
            return StatusCode(500, new { success = false, message = "Error al verificar estado" });
        }
    }
}
