using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class ContactanosModel : PageModel
    {
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
        [Required(ErrorMessage = "El mensaje es obligatorio")]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres")]
        [MinLength(10, ErrorMessage = "El mensaje debe tener al menos 10 caracteres")]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        public void OnGet()
        {
            // Inicialización de la página si es necesario
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Aquí se procesaría el mensaje de contacto
                await ProcessContactMessageAsync();

                TempData["SuccessMessage"] = "¡Gracias por contactarnos! Su mensaje ha sido enviado correctamente. Nos pondremos en contacto con usted pronto.";
                
                // Limpiar el formulario después del envío exitoso
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log del error (en un escenario real)
                // _logger.LogError(ex, "Error processing contact message");

                TempData["ErrorMessage"] = "Ocurrió un error al enviar su mensaje. Por favor, inténtelo nuevamente.";
                return Page();
            }
        }

        private async Task ProcessContactMessageAsync()
        {
            // Simular procesamiento asíncrono
            await Task.Delay(100);

            // En un escenario real, aquí se podría:
            // 1. Guardar el mensaje en una base de datos
            // 2. Enviar notificación por correo electrónico al equipo de soporte
            // 3. Enviar confirmación por correo al usuario
            // 4. Integrar con un sistema de tickets/CRM

            // Ejemplo de datos que se procesarían:
            var contactData = new
            {
                Nombre = this.Nombre,
                Apellido = this.Apellido,
                Email = this.Email,
                TipoConsulta = this.TipoConsulta,
                Telefono = this.Telefono,
                Mensaje = this.Mensaje,
                FechaEnvio = DateTime.Now,
                DireccionIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            // Simular guardado en base de datos o envío de email
        }
    }
}