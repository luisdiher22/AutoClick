using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using AutoClick.Services;

namespace AutoClick.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationInsightsService _appInsightsService;

        public DashboardModel(
            ApplicationDbContext context,
            IApplicationInsightsService appInsightsService)
        {
            _context = context;
            _appInsightsService = appInsightsService;
        }

        // Estadísticas de visitas (Application Insights)
        public string VisitasMensuales { get; set; } = "0";
        public string VisitasSemanales { get; set; } = "0";
        public string VisitasHoy { get; set; } = "0";
        
        // Anuncios - Datos reales de la base de datos
        public int TotalAutos { get; set; }
        public int AutosActivos { get; set; }
        public int AutosInactivos { get; set; }
        
        // Mensajes - Datos reales de la base de datos
        public int TotalMensajes { get; set; }
        public int MensajesHoy { get; set; }
        public int MensajesEstaSemana { get; set; }
        
        // Datos adicionales
        public int TotalUsuarios { get; set; }
        public DateTime? UltimaActualizacion { get; set; }

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
                UltimaActualizacion = DateTime.Now;

                // ===== ESTADÍSTICAS DE VISITAS (APPLICATION INSIGHTS) =====
                // Obtener métricas reales de Application Insights
                var visitasMensuales = await _appInsightsService.GetMonthlyPageViewsAsync();
                var visitasSemanales = await _appInsightsService.GetWeeklyPageViewsAsync();
                var visitasHoy = await _appInsightsService.GetDailyPageViewsAsync();
                
                // Formatear números (K para miles, M para millones)
                VisitasMensuales = FormatNumber(visitasMensuales);
                VisitasSemanales = FormatNumber(visitasSemanales);
                VisitasHoy = FormatNumber(visitasHoy);

                // ===== ESTADÍSTICAS DE ANUNCIOS (TABLA AUTOS) =====
                TotalAutos = await _context.Autos.AsNoTracking().CountAsync();
                
                // Autos activos: aquellos que tienen la bandera Activo = true
                AutosActivos = await _context.Autos
                    .AsNoTracking()
                    .Where(a => a.Activo)
                    .CountAsync();
                
                // Autos inactivos: desactivados o eliminados
                AutosInactivos = TotalAutos - AutosActivos;

                // ===== ESTADÍSTICAS DE MENSAJES (TABLA MENSAJES) =====
                TotalMensajes = await _context.Mensajes.AsNoTracking().CountAsync();
                
                // Mensajes de hoy
                var hoy = DateTime.Today;
                MensajesHoy = await _context.Mensajes
                    .AsNoTracking()
                    .Where(m => m.FechaCreacion.Date == hoy)
                    .CountAsync();
                
                // Mensajes de esta semana (últimos 7 días)
                var hace7Dias = DateTime.Now.AddDays(-7);
                MensajesEstaSemana = await _context.Mensajes
                    .AsNoTracking()
                    .Where(m => m.FechaCreacion >= hace7Dias)
                    .CountAsync();

                // Total de usuarios registrados
                TotalUsuarios = await _context.Usuarios.AsNoTracking().CountAsync();
            }
            catch (Exception ex)
            {
                // En caso de error, inicializar con valores por defecto
                TotalUsuarios = 0;
                TotalAutos = 0;
                AutosActivos = 0;
                AutosInactivos = 0;
                TotalMensajes = 0;
                MensajesHoy = 0;
                MensajesEstaSemana = 0;
                
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error en Dashboard: {ex.Message}");
            }

            return Page();
        }

        /// <summary>
        /// Formatea un número grande a formato legible (K para miles, M para millones)
        /// </summary>
        private string FormatNumber(int number)
        {
            if (number >= 1000000)
            {
                return $"{(number / 1000000.0):F1}M";
            }
            else if (number >= 1000)
            {
                return $"{(number / 1000.0):F1}K";
            }
            else
            {
                return number.ToString();
            }
        }
    }
}
