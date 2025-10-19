using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Estadísticas generales
        public string VisitasMensuales { get; set; } = "159.4K";
        public string VisitasOrganicas { get; set; } = "80%";
        public string VisitasPagadas { get; set; } = "20%";
        
        // Anuncios
        public int TotalAutos { get; set; }
        public int PosteoInstantaneo { get; set; } = 221;
        public int PendientesAprobacion { get; set; } = 14;
        
        // Solicitudes de fotografía
        public int SolicitudesAgendadas { get; set; } = 15;
        public int SolicitudesPendientes { get; set; } = 8;
        public int SolicitudesFinalizadas { get; set; } = 7;
        
        // Mensajes
        public int TotalMensajes { get; set; }
        public int MensajesPendientes { get; set; } = 13;
        public int MensajesRespondidos { get; set; } = 2;
        
        // Datos adicionales (para futuro uso)
        public int TotalUsuarios { get; set; }
        public int TotalReclamos { get; set; }
        public List<Usuario> UsuariosRecientes { get; set; } = new();
        public List<Auto> AutosRecientes { get; set; } = new();
        public List<Mensaje> MensajesRecientes { get; set; } = new();
        public List<Reclamo> ReclamosRecientes { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar que el usuario sea administrador
            var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
            if (!isAdmin)
            {
                return Forbid();
            }

            try
            {
                // Obtener estadísticas generales (sin tracking para mejor performance)
                TotalUsuarios = await _context.Usuarios.AsNoTracking().CountAsync();
                TotalAutos = await _context.Autos.AsNoTracking().CountAsync();
                TotalMensajes = await _context.Mensajes.AsNoTracking().CountAsync();
                TotalReclamos = await _context.Reclamos.AsNoTracking().CountAsync();

                // Obtener datos recientes (sin tracking y con proyección)
                UsuariosRecientes = await _context.Usuarios
                    .AsNoTracking()
                    .OrderBy(u => u.Email)
                    .Take(5)
                    .ToListAsync();

                AutosRecientes = await _context.Autos
                    .AsNoTracking()
                    .OrderByDescending(a => a.Id)
                    .Take(5)
                    .ToListAsync();

                MensajesRecientes = await _context.Mensajes
                    .AsNoTracking()
                    .Include(m => m.Cliente)
                    .OrderByDescending(m => m.FechaCreacion)
                    .Take(5)
                    .ToListAsync();

                ReclamosRecientes = await _context.Reclamos
                    .AsNoTracking()
                    .Include(r => r.Cliente)
                    .OrderByDescending(r => r.FechaCreacion)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // En caso de error, inicializar con valores por defecto
                TotalUsuarios = 0;
                TotalAutos = 0;
                TotalMensajes = 0;
                TotalReclamos = 0;
                
                // Log del error en un entorno de producción
                System.Diagnostics.Debug.WriteLine($"Error en Dashboard: {ex.Message}");
            }

            return Page();
        }
    }
}