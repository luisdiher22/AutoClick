using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;
using AutoClick.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages.Admin
{
    [Authorize] // Solo usuarios autenticados pueden acceder
    public class CentroSoporteModel : PageModel
    {
        private readonly ISoporteService _soporteService;
        private readonly IAuthService _authService;

        public CentroSoporteModel(ISoporteService soporteService, IAuthService authService)
        {
            _soporteService = soporteService;
            _authService = authService;
        }

        // Propiedades para la vista
        public List<Reclamo> Reclamos { get; set; } = new();
        public List<Mensaje> Mensajes { get; set; } = new();
        
        // Paginación
        [BindProperty(SupportsGet = true)]
        public int PaginaActualReclamos { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PaginaActualMensajes { get; set; } = 1;
        
        public int TotalPaginasReclamos { get; set; }
        public int TotalPaginasMensajes { get; set; }
        
        private const int TamañoPagina = 15;
        
        // Filtros
        [BindProperty(SupportsGet = true)]
        public string? FiltroTipoReclamo { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public EstadoReclamo? FiltroEstadoReclamo { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? FiltroTipoMensaje { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public EstadoMensaje? FiltroEstadoMensaje { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? FiltroPrioridad { get; set; }

        // Estadísticas para el dashboard
        public Dictionary<string, int> EstadisticasReclamos { get; set; } = new();
        public Dictionary<string, int> EstadisticasMensajes { get; set; } = new();

        // Listas para los selectores
        public List<string> TiposProblemaDisponibles { get; set; } = new();
        public List<string> TiposConsultaDisponibles { get; set; } = new();
        public List<string> PrioridadesDisponibles { get; set; } = new();

        // Propiedades para responder
        [BindProperty]
        public int ItemId { get; set; }
        
        [BindProperty]
        public string TipoItem { get; set; } = ""; // "reclamo" o "mensaje"
        
        [BindProperty]
        [Required(ErrorMessage = "La respuesta es obligatoria")]
        [StringLength(1000, ErrorMessage = "La respuesta no puede exceder 1000 caracteres")]
        public string RespuestaTexto { get; set; } = "";

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Verificar que el usuario sea administrador
                var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
                if (!isAdmin)
                {
                    return Forbid();
                }

                await CargarDatosAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cargar los datos del centro de soporte.";
                Console.WriteLine($"Error en CentroSoporte OnGet: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResponderAsync()
        {
            try
            {
                // Verificar que el usuario sea administrador
                var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
                if (!isAdmin)
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    await CargarDatosAsync();
                    ErrorMessage = "Por favor, corrija los errores en el formulario.";
                    return Page();
                }

                var emailAdmin = User.Identity?.Name ?? "";
                bool resultado = false;

                if (TipoItem == "reclamo")
                {
                    resultado = await _soporteService.ResponderReclamoAsync(ItemId, RespuestaTexto, emailAdmin);
                }
                else if (TipoItem == "mensaje")
                {
                    resultado = await _soporteService.ResponderMensajeAsync(ItemId, RespuestaTexto, emailAdmin);
                }

                if (resultado)
                {
                    SuccessMessage = "Respuesta enviada exitosamente.";
                    
                    // Limpiar formulario
                    ItemId = 0;
                    TipoItem = "";
                    RespuestaTexto = "";
                }
                else
                {
                    ErrorMessage = "Error al enviar la respuesta.";
                }

                await CargarDatosAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al procesar la respuesta.";
                Console.WriteLine($"Error en CentroSoporte OnPostResponder: {ex.Message}");
                await CargarDatosAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCambiarEstadoAsync(int id, string tipo, string nuevoEstado)
        {
            try
            {
                // Verificar que el usuario sea administrador
                var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
                if (!isAdmin)
                {
                    return Forbid();
                }

                bool resultado = false;

                if (tipo == "reclamo" && Enum.TryParse<EstadoReclamo>(nuevoEstado, out var estadoReclamo))
                {
                    resultado = await _soporteService.CambiarEstadoReclamoAsync(id, estadoReclamo);
                }
                else if (tipo == "mensaje" && Enum.TryParse<EstadoMensaje>(nuevoEstado, out var estadoMensaje))
                {
                    resultado = await _soporteService.CambiarEstadoMensajeAsync(id, estadoMensaje);
                }

                if (resultado)
                {
                    SuccessMessage = "Estado actualizado exitosamente.";
                }
                else
                {
                    ErrorMessage = "Error al actualizar el estado.";
                }

                await CargarDatosAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cambiar el estado.";
                Console.WriteLine($"Error en CentroSoporte OnPostCambiarEstado: {ex.Message}");
                await CargarDatosAsync();
                return Page();
            }
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                // Cargar listas para los filtros
                TiposProblemaDisponibles = await _soporteService.GetTiposProblemaAsync();
                TiposConsultaDisponibles = await _soporteService.GetTiposConsultaAsync();
                PrioridadesDisponibles = _soporteService.GetPrioridades();

                // Cargar reclamos con filtros
                List<Reclamo> todosLosReclamos;
                if (!string.IsNullOrEmpty(FiltroTipoReclamo) || FiltroEstadoReclamo.HasValue || !string.IsNullOrEmpty(FiltroPrioridad))
                {
                    todosLosReclamos = await _soporteService.GetReclamosPorFiltroAsync(
                        FiltroTipoReclamo, 
                        FiltroEstadoReclamo, 
                        FiltroPrioridad);
                }
                else
                {
                    todosLosReclamos = await _soporteService.GetReclamosAsync();
                }
                
                // Ordenar por fecha descendente (más reciente primero)
                todosLosReclamos = todosLosReclamos.OrderByDescending(r => r.FechaCreacion).ToList();
                
                // Calcular paginación para reclamos
                var totalReclamos = todosLosReclamos.Count;
                TotalPaginasReclamos = (int)Math.Ceiling(totalReclamos / (double)TamañoPagina);
                PaginaActualReclamos = Math.Max(1, Math.Min(PaginaActualReclamos, TotalPaginasReclamos == 0 ? 1 : TotalPaginasReclamos));
                
                // Aplicar paginación
                Reclamos = todosLosReclamos
                    .Skip((PaginaActualReclamos - 1) * TamañoPagina)
                    .Take(TamañoPagina)
                    .ToList();

                // Cargar mensajes con filtros
                List<Mensaje> todosLosMensajes;
                if (!string.IsNullOrEmpty(FiltroTipoMensaje) || FiltroEstadoMensaje.HasValue || !string.IsNullOrEmpty(FiltroPrioridad))
                {
                    todosLosMensajes = await _soporteService.GetMensajesPorFiltroAsync(
                        FiltroTipoMensaje, 
                        FiltroEstadoMensaje, 
                        FiltroPrioridad);
                }
                else
                {
                    todosLosMensajes = await _soporteService.GetMensajesAsync();
                }
                
                // Ordenar por fecha descendente (más reciente primero)
                todosLosMensajes = todosLosMensajes.OrderByDescending(m => m.FechaCreacion).ToList();
                
                // Calcular paginación para mensajes
                var totalMensajes = todosLosMensajes.Count;
                TotalPaginasMensajes = (int)Math.Ceiling(totalMensajes / (double)TamañoPagina);
                PaginaActualMensajes = Math.Max(1, Math.Min(PaginaActualMensajes, TotalPaginasMensajes == 0 ? 1 : TotalPaginasMensajes));
                
                // Aplicar paginación
                Mensajes = todosLosMensajes
                    .Skip((PaginaActualMensajes - 1) * TamañoPagina)
                    .Take(TamañoPagina)
                    .ToList();

                // Cargar estadísticas
                EstadisticasReclamos = await _soporteService.GetEstadisticasReclamosAsync();
                EstadisticasMensajes = await _soporteService.GetEstadisticasMensajesAsync();
            }
            catch (Exception ex)
            {
                // Inicializar valores por defecto en caso de error
                TiposProblemaDisponibles = new List<string>();
                TiposConsultaDisponibles = new List<string>();
                PrioridadesDisponibles = new List<string> { "Baja", "Media", "Alta", "Crítica" };
                Reclamos = new List<Reclamo>();
                Mensajes = new List<Mensaje>();
                EstadisticasReclamos = new Dictionary<string, int>();
                EstadisticasMensajes = new Dictionary<string, int>();
                TotalPaginasReclamos = 1;
                TotalPaginasMensajes = 1;
                
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
        }

        // Métodos helper para la vista
        public string GetPrioridadClass(string prioridad)
        {
            return prioridad?.ToLower() switch
            {
                "crítica" => "priority-critical",
                "alta" => "priority-high", 
                "media" => "priority-medium",
                "baja" => "priority-low",
                _ => "priority-medium"
            };
        }

        public string GetEstadoClass(string estado)
        {
            return estado?.ToLower() switch
            {
                "pendiente" => "status-pending",
                "no leído" => "status-unread",
                "en proceso" => "status-in-progress", 
                "leído" => "status-read",
                "resuelto" => "status-resolved",
                "respondido" => "status-responded",
                "cerrado" => "status-closed",
                "archivado" => "status-archived",
                _ => "status-default"
            };
        }
    }
}