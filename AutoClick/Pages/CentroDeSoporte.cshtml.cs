using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;
using AutoClick.Models;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    [Authorize] // Solo usuarios autenticados pueden acceder
    public class CentroDeSoporteModel : PageModel
    {
        private readonly ISoporteService _soporteService;
        private readonly IAuthService _authService;

        public CentroDeSoporteModel(ISoporteService soporteService, IAuthService authService)
        {
            _soporteService = soporteService;
            _authService = authService;
        }

        // Propiedades para la vista
        public List<Reclamo> Reclamos { get; set; } = new();
        public List<Mensaje> Mensajes { get; set; } = new();
        
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
                // Verificar si el usuario es administrador (opcional - puedes ajustar esta lógica)
                var currentUser = await _authService.GetUserByEmailAsync(User.Identity?.Name ?? "");
                if (currentUser == null || currentUser.Email != "admin@gmail.com")
                {
                    // Solo admins pueden ver el centro de soporte por ahora
                    return Forbid();
                }

                await CargarDatosAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cargar los datos del centro de soporte.";
                Console.WriteLine($"Error en CentroDeSoporte OnGet: {ex.Message}");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResponderAsync()
        {
            try
            {
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
                Console.WriteLine($"Error en CentroDeSoporte OnPostResponder: {ex.Message}");
                await CargarDatosAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCambiarEstadoAsync(int id, string tipo, string nuevoEstado)
        {
            try
            {
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
                Console.WriteLine($"Error en CentroDeSoporte OnPostCambiarEstado: {ex.Message}");
                await CargarDatosAsync();
                return Page();
            }
        }

        private async Task CargarDatosAsync()
        {
            // Cargar listas para los filtros
            TiposProblemaDisponibles = await _soporteService.GetTiposProblemaAsync();
            TiposConsultaDisponibles = await _soporteService.GetTiposConsultaAsync();
            PrioridadesDisponibles = _soporteService.GetPrioridades();

            // Cargar reclamos con filtros
            if (!string.IsNullOrEmpty(FiltroTipoReclamo) || FiltroEstadoReclamo.HasValue || !string.IsNullOrEmpty(FiltroPrioridad))
            {
                Reclamos = await _soporteService.GetReclamosPorFiltroAsync(
                    FiltroTipoReclamo, 
                    FiltroEstadoReclamo, 
                    FiltroPrioridad);
            }
            else
            {
                Reclamos = await _soporteService.GetReclamosAsync();
            }

            // Cargar mensajes con filtros
            if (!string.IsNullOrEmpty(FiltroTipoMensaje) || FiltroEstadoMensaje.HasValue || !string.IsNullOrEmpty(FiltroPrioridad))
            {
                Mensajes = await _soporteService.GetMensajesPorFiltroAsync(
                    FiltroTipoMensaje, 
                    FiltroEstadoMensaje, 
                    FiltroPrioridad);
            }
            else
            {
                Mensajes = await _soporteService.GetMensajesAsync();
            }

            // Cargar estadísticas
            EstadisticasReclamos = await _soporteService.GetEstadisticasReclamosAsync();
            EstadisticasMensajes = await _soporteService.GetEstadisticasMensajesAsync();
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