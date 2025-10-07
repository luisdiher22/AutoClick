using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class PpfCeramicoModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "El teléfono es requerido")]
        [RegularExpression(@"^\+?506\s?\d{4}\s?\d{4}$|^\d{4}\s?\d{4}$", ErrorMessage = "Formato de teléfono inválido. Use: 8888-8888 o +506 8888-8888")]
        public string Telefono { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [BindProperty, Required(ErrorMessage = "Debe seleccionar un tipo de vehículo")]
        public string TipoVehiculo { get; set; } = string.Empty;

        [BindProperty]
        public List<string> ServicioInteres { get; set; } = new List<string>();

        [BindProperty]
        [StringLength(1000, ErrorMessage = "El mensaje no puede exceder 1000 caracteres")]
        public string? Mensaje { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Limpiar mensajes al cargar la página
            ErrorMessage = null;
            SuccessMessage = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor complete todos los campos requeridos correctamente.";
                return Page();
            }

            try
            {
                // Validar que se haya seleccionado al menos un servicio
                if (ServicioInteres == null || !ServicioInteres.Any())
                {
                    ModelState.AddModelError(nameof(ServicioInteres), "Debe seleccionar al menos un servicio de interés");
                    ErrorMessage = "Debe seleccionar al menos un servicio de interés.";
                    return Page();
                }

                // Simular procesamiento de la cotización
                await Task.Delay(1000); // Simula operación asíncrona

                // Log the request (en una implementación real, esto se guardaría en base de datos)
                var serviciosSeleccionados = string.Join(", ", ServicioInteres);
                
                // Aquí normalmente se:
                // 1. Guardaría la solicitud en base de datos
                // 2. Enviaría email de confirmación al cliente
                // 3. Notificaría al equipo de ventas
                
                // Simular éxito
                SuccessMessage = $"¡Gracias {Nombre}! Hemos recibido su solicitud de cotización para {serviciosSeleccionados}. " +
                               "Nos pondremos en contacto con usted en las próximas 24 horas al {Telefono}.";

                // Limpiar el formulario después del envío exitoso
                ModelState.Clear();
                Nombre = string.Empty;
                Telefono = string.Empty;
                Email = string.Empty;
                TipoVehiculo = string.Empty;
                ServicioInteres = new List<string>();
                Mensaje = string.Empty;

                return Page();
            }
            catch (Exception ex)
            {
                // En un entorno de producción, loggear la excepción
                ErrorMessage = "Ocurrió un error al procesar su solicitud. Por favor intente nuevamente o contáctenos directamente.";
                return Page();
            }
        }
    }
}