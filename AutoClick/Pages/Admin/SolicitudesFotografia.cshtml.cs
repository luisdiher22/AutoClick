using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.Pages.Admin
{
    public class SolicitudesFotografiaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 15;

        public SolicitudesFotografiaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SolicitudFotografiaItem> Solicitudes { get; set; } = new List<SolicitudFotografiaItem>();
        
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;
        
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public async Task OnGetAsync()
        {
            // Contar total de solicitudes (Plan nivel 4)
            TotalRegistros = await _context.Autos
                .Where(a => a.PlanVisibilidad == 4 && a.Activo)
                .CountAsync();

            // Calcular total de páginas
            TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)PageSize);

            // Asegurar que la página actual esté en rango válido
            if (PaginaActual < 1) PaginaActual = 1;
            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;

            // Obtener solicitudes con paginación
            Solicitudes = await _context.Autos
                .Where(a => a.PlanVisibilidad == 4 && a.Activo)
                .Include(a => a.Propietario) // Incluir datos del propietario
                .OrderByDescending(a => a.FechaCreacion)
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new SolicitudFotografiaItem
                {
                    Id = a.Id,
                    Code = $"#{a.Id}",
                    Title = a.NombreCompleto,
                    Date = a.FechaCreacion.ToString("dd/MM/yyyy"),
                    EstadoAgenda = a.EstadoAgendaFotografia,
                    // Información del propietario
                    NombrePropietario = a.Propietario != null ? a.Propietario.NombreCompleto : "N/A",
                    EmailPropietario = a.EmailPropietario,
                    TelefonoPropietario = a.Propietario != null ? a.Propietario.NumeroTelefono : "N/A",
                    UbicacionVehiculo = a.UbicacionCompleta
                })
                .ToListAsync();
        }

        // Endpoint para cambiar el estado de la agenda
        public async Task<IActionResult> OnPostCambiarEstadoAsync([FromForm] int id, [FromForm] string nuevoEstado)
        {
            try
            {
                var auto = await _context.Autos.FindAsync(id);
                if (auto == null)
                {
                    return new JsonResult(new { success = false, message = "Vehículo no encontrado" });
                }
                
                auto.EstadoAgendaFotografia = nuevoEstado;
                auto.FechaActualizacion = DateTime.UtcNow;
                
                // Marcar explícitamente como modificado
                _context.Entry(auto).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }

    public class SolicitudFotografiaItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string EstadoAgenda { get; set; } = "Sin agendar";
        
        // Información del propietario
        public string NombrePropietario { get; set; } = string.Empty;
        public string EmailPropietario { get; set; } = string.Empty;
        public string TelefonoPropietario { get; set; } = string.Empty;
        public string UbicacionVehiculo { get; set; } = string.Empty;
    }
}
