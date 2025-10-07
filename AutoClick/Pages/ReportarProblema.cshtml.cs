using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class ReportarProblemaModel : PageModel
    {
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
        [Required(ErrorMessage = "Debe seleccionar un tipo de problema")]
        [Display(Name = "Tipo de problema")]
        public string TipoProblema { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La descripción del problema es obligatoria")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        [Display(Name = "Descripción del problema")]
        public string DescripcionProblema { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // Initialize page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor, corrija los errores en el formulario.";
                return Page();
            }

            try
            {
                // Simulate processing the problem report
                await ProcessProblemReportAsync();

                SuccessMessage = "Su reporte ha sido enviado exitosamente. Nos pondremos en contacto con usted pronto.";
                
                // Clear form data after successful submission
                ClearFormData();
                
                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "Ocurrió un error al enviar su reporte. Por favor, inténtelo nuevamente.";
                return Page();
            }
        }

        private async Task ProcessProblemReportAsync()
        {
            // In a real application, you would:
            // 1. Save to database
            // 2. Send email notification to support team
            // 3. Send confirmation email to user
            // 4. Log the issue for tracking

            // Simulate async operation
            await Task.Delay(100);

            // Example of what you might do:
            /*
            var problemReport = new ProblemReport
            {
                Nombre = Nombre,
                Apellido = Apellido,
                Email = Email,
                Telefono = Telefono,
                TipoProblema = TipoProblema,
                DescripcionProblema = DescripcionProblema,
                FechaReporte = DateTime.Now,
                Estado = "Pendiente"
            };

            await _dbContext.ProblemReports.AddAsync(problemReport);
            await _dbContext.SaveChangesAsync();

            // Send email to support team
            await _emailService.SendProblemReportNotificationAsync(problemReport);
            
            // Send confirmation email to user
            await _emailService.SendProblemReportConfirmationAsync(problemReport);
            */
        }

        private void ClearFormData()
        {
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            ConfirmarEmail = string.Empty;
            TipoProblema = string.Empty;
            Telefono = string.Empty;
            DescripcionProblema = string.Empty;
        }
    }
}