using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Services;
using AutoClick.Models;

namespace AutoClick.Pages
{
    public class ReportarProblemaModel : PageModel
    {
        private readonly ISoporteService _soporteService;

        public ReportarProblemaModel(ISoporteService soporteService)
        {
            _soporteService = soporteService;
        }
        [BindProperty]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar su correo electrónico")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [Compare("Email", ErrorMessage = "Los correos electrónicos no coinciden")]
        [Display(Name = "Confirmar correo electrónico")]
        public string ConfirmarEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debe seleccionar un tipo de problema")]
        [Display(Name = "Tipo de problema")]
        public string TipoProblema { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(100, ErrorMessage = "El asunto no puede exceder 100 caracteres")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La descripción del problema es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [Display(Name = "Descripción del problema")]
        public string DescripcionProblema { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        
        public List<string> TiposProblemaDisponibles { get; set; } = new();

        public async Task OnGetAsync()
        {
            TiposProblemaDisponibles = await _soporteService.GetTiposProblemaAsync();
            
            // Leer mensaje de éxito de TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Recargar tipos de problema para la vista en caso de error
            TiposProblemaDisponibles = await _soporteService.GetTiposProblemaAsync();
            
            // Remover errores del campo Asunto del ModelState ya que se genera automáticamente
            ModelState.Remove(nameof(Asunto));
            
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, corrija los errores en el formulario.";
                return Page();
            }

            try
            {
                await ProcessProblemReportAsync();

                TempData["SuccessMessage"] = "Su reporte ha sido enviado exitosamente. Nos pondremos en contacto con usted pronto.";
                
                // Usar patrón Post-Redirect-Get para limpiar el formulario
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ocurrió un error al enviar su reporte. Por favor, inténtelo nuevamente.";

                return Page();
            }
        }

        private async Task ProcessProblemReportAsync()
        {
            // Generar asunto automáticamente basado en el tipo de problema
            var asuntoGenerado = GenerarAsuntoAutomatico(TipoProblema);
            
            var reclamo = new Reclamo
            {
                EmailCliente = Email,
                Nombre = Nombre,
                Apellidos = Apellido,
                Telefono = Telefono,
                TipoProblema = TipoProblema,
                Asunto = asuntoGenerado,
                Descripcion = DescripcionProblema,
                Prioridad = "Media"
            };

            var reclamoId = await _soporteService.CrearReclamoAsync(reclamo);
            
            if (reclamoId == 0)
            {
                throw new Exception("Error al guardar el reclamo");
            }
        }

        private void ClearFormData()
        {
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            ConfirmarEmail = string.Empty;
            Telefono = string.Empty;
            TipoProblema = string.Empty;
            Asunto = string.Empty;
            DescripcionProblema = string.Empty;
        }

        private string GenerarAsuntoAutomatico(string tipoProblema)
        {
            return tipoProblema switch
            {
                "Problema técnico del sitio" => "Problema técnico del sitio",
                "Problema con mi cuenta" => "Problema con la cuenta", 
                "Problema con anuncio" => "Problema con anuncio",
                "Problema de pagos y facturación" => "Problema con pago y facturación",
                "Otro problema" => "Incidencia reportada",
                _ => "Incidencia reportada - AutoClick.cr"
            };
        }
    }
}