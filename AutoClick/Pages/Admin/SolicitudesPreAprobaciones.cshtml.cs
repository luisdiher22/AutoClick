using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoClick.Pages.Admin
{
    public class SolicitudesPreAprobacionesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 20;

        public SolicitudesPreAprobacionesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SolicitudPreAprobacionViewModel> Solicitudes { get; set; } = new();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? paginaActual)
        {
            try
            {
                PaginaActual = paginaActual ?? 1;

                // Solo mostrar solicitudes NO procesadas (pendientes)
                var query = _context.SolicitudesPreAprobacion
                    .Include(s => s.Auto)
                    .Where(s => !s.Procesada)
                    .OrderByDescending(s => s.FechaSolicitud)
                    .AsQueryable();

                var totalSolicitudes = await query.CountAsync();
                TotalPaginas = (int)Math.Ceiling(totalSolicitudes / (double)PageSize);

                var solicitudes = await query
                    .Skip((PaginaActual - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                Solicitudes = solicitudes.Select(s => new SolicitudPreAprobacionViewModel
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Apellidos = s.Apellidos,
                    Telefono = s.Telefono,
                    Email = s.Email,
                    FechaSolicitud = s.FechaSolicitud,
                    AutoId = s.AutoId,
                    AutoNombre = s.Auto?.NombreCompleto ?? "Auto no disponible",
                    Procesada = s.Procesada
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cargar las solicitudes de pre-aprobación.";

                return Page();
            }
        }

        public async Task<IActionResult> OnPostAprobarAsync(int solicitudId)
        {
            try
            {
                Console.WriteLine($"[APROBAR] Iniciando aprobación de solicitud ID: {solicitudId}");
                
                var solicitud = await _context.SolicitudesPreAprobacion
                    .FirstOrDefaultAsync(s => s.Id == solicitudId);
                    
                if (solicitud != null)
                {
                    Console.WriteLine($"[APROBAR] Solicitud encontrada. AutoId: {solicitud.AutoId}");
                    
                    // Marcar la solicitud como procesada y aprobada
                    solicitud.Procesada = true;
                    solicitud.FechaProcesamiento = DateTime.Now;
                    solicitud.Aprobada = true;
                    
                    // Cargar y activar el auto asociado DIRECTAMENTE (no via Include)
                    var auto = await _context.Autos.FindAsync(solicitud.AutoId);
                    if (auto != null)
                    {
                        Console.WriteLine($"[APROBAR] Auto encontrado. Activo antes: {auto.Activo}");
                        auto.Activo = true;
                        // Marcar explícitamente como modificado para que EF detecte el cambio
                        _context.Entry(auto).State = EntityState.Modified;
                        Console.WriteLine($"[APROBAR] Auto.Activo establecido a: {auto.Activo}, Estado: {_context.Entry(auto).State}");
                    }
                    else
                    {
                        Console.WriteLine($"[APROBAR] ADVERTENCIA: Auto con ID {solicitud.AutoId} no encontrado");
                    }
                    
                    // Marcar solicitud como modificada también
                    _context.Entry(solicitud).State = EntityState.Modified;
                    
                    var cambios = await _context.SaveChangesAsync();
                    Console.WriteLine($"[APROBAR] Cambios guardados: {cambios} registros afectados");
                }
                else
                {
                    Console.WriteLine($"[APROBAR] ERROR: No se encontró solicitud con ID {solicitudId}");
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[APROBAR] EXCEPCIÓN: {ex.Message}");
                Console.WriteLine($"[APROBAR] StackTrace: {ex.StackTrace}");
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostRechazarAsync(int solicitudId)
        {
            try
            {
                var solicitud = await _context.SolicitudesPreAprobacion
                    .Include(s => s.Auto)
                    .FirstOrDefaultAsync(s => s.Id == solicitudId);
                    
                if (solicitud != null)
                {
                    // Si hay un auto asociado, eliminarlo
                    if (solicitud.Auto != null)
                    {
                        // IMPORTANTE: Primero eliminar los pagos asociados al auto (FK constraint)
                        var pagosAsociados = await _context.PagosOnvo
                            .Where(p => p.AutoId == solicitud.Auto.Id)
                            .ToListAsync();
                        if (pagosAsociados.Any())
                        {
                            _context.PagosOnvo.RemoveRange(pagosAsociados);
                        }
                        
                        // Ahora sí eliminar el auto
                        _context.Autos.Remove(solicitud.Auto);
                    }
                    
                    // Eliminar la solicitud también
                    _context.SolicitudesPreAprobacion.Remove(solicitud);
                    
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error al rechazar solicitud: {ex.Message}");
                return RedirectToPage();
            }
        }
    }

    public class SolicitudPreAprobacionViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public int AutoId { get; set; }
        public string AutoNombre { get; set; } = string.Empty;
        public bool Procesada { get; set; }
    }
}
