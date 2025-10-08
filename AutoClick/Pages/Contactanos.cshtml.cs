using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Services;
using AutoClick.Models;

namespace AutoClick.Pages
{
    public class ContactanosModel : PageModel
    {
        private readonly ISoporteService _soporteService;

        public ContactanosModel(ISoporteService soporteService)
        {
            _soporteService = soporteService;
        }
        [BindProperty]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduzca un correo electrónico válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La confirmación del correo electrónico es obligatoria")]
        [EmailAddress(ErrorMessage = "Por favor, introduzca un correo electrónico válido")]
        [Compare(nameof(Email), ErrorMessage = "Los correos electrónicos no coinciden")]
        [Display(Name = "Confirmar correo electrónico")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El tipo de consulta es obligatorio")]
        [Display(Name = "Tipo de consulta")]
        public string TipoConsulta { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El número de teléfono es obligatorio")]
        [Phone(ErrorMessage = "Por favor, introduzca un número de teléfono válido")]
        [StringLength(20, ErrorMessage = "El número de teléfono no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(100, ErrorMessage = "El asunto no puede exceder 100 caracteres")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres")]
        [MinLength(10, ErrorMessage = "El mensaje debe tener al menos 10 caracteres")]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public List<string> TiposConsultaDisponibles { get; set; } = new();

        public async Task OnGetAsync()
        {
            TiposConsultaDisponibles = await _soporteService.GetTiposConsultaAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Recargar tipos de consulta para la vista en caso de error
            TiposConsultaDisponibles = await _soporteService.GetTiposConsultaAsync();
            
            // Remover errores del campo Asunto del ModelState ya que se genera automáticamente
            ModelState.Remove(nameof(Asunto));
            
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, corrija los errores en el formulario.";
                return Page();
            }

            try
            {
                await ProcessContactMessageAsync();

                SuccessMessage = "Su consulta ha sido enviada exitosamente. Nos pondremos en contacto con usted pronto.";
                
                // Limpiar el formulario después del envío exitoso
                ClearFormData();
                
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error al enviar su consulta: {ex.Message}";
                Console.WriteLine($"Error en Contactanos: {ex}");
                return Page();
            }
        }

        private async Task ProcessContactMessageAsync()
        {
            // Validar que todos los campos requeridos tengan valores
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Nombre) || 
                string.IsNullOrEmpty(Apellido) || string.IsNullOrEmpty(TipoConsulta) || 
                string.IsNullOrEmpty(Mensaje))
            {
                throw new ArgumentException("Faltan campos requeridos para procesar la consulta");
            }

            // Generar asunto automáticamente basado en el tipo de consulta
            var asuntoGenerado = GenerarAsuntoAutomatico(TipoConsulta);
            
            if (string.IsNullOrEmpty(asuntoGenerado))
            {
                throw new ArgumentException("No se pudo generar el asunto automáticamente");
            }
            
            var mensaje = new Mensaje
            {
                EmailCliente = Email?.Trim() ?? string.Empty,
                Nombre = Nombre?.Trim() ?? string.Empty,
                Apellidos = Apellido?.Trim() ?? string.Empty,
                TipoConsulta = TipoConsulta?.Trim() ?? string.Empty,
                Asunto = asuntoGenerado,
                ContenidoMensaje = this.Mensaje?.Trim() ?? string.Empty,
                Telefono = Telefono?.Trim() ?? string.Empty,
                Prioridad = "Media"
            };

            var mensajeId = await _soporteService.CrearMensajeAsync(mensaje);
            
            if (mensajeId == 0)
            {
                throw new Exception("Error al guardar el mensaje");
            }
        }

        private string GenerarAsuntoAutomatico(string tipoConsulta)
        {
            return tipoConsulta switch
            {
                "Consulta General" => "Consulta general",
                "Soporte Técnico" => "Soporte técnico",
                "Publicar Anuncio" => "Publicar anuncio",
                "Problema con Anuncio" => "Problema con anuncio",
                "Facturación" => "Consulta de facturación",
                "Sugerencias" => "Sugerencias",
                _ => "Consulta - AutoClick.cr"
            };
        }

        private void ClearFormData()
        {
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            ConfirmEmail = string.Empty;
            Telefono = string.Empty;
            TipoConsulta = string.Empty;
            Asunto = string.Empty;
            Mensaje = string.Empty;
        }
    }
}