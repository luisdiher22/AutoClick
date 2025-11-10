using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class SobreNosotrosModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor ingrese un correo electrónico válido")]
        public string Email { get; set; } = string.Empty;

        [TempData]
        public string? StatusMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Page initialization if needed
        }

        public async Task<IActionResult> OnPostNewsletterAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Por favor corrija los errores en el formulario";
                    return Page();
                }

                // Validate email format
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "El correo electrónico es obligatorio";
                    return Page();
                }

                // Here you would typically:
                // 1. Add email to newsletter subscription database
                // 2. Send welcome email
                // 3. Integrate with email marketing service (MailChimp, SendGrid, etc.)
                
                // For now, we'll simulate the process
                await Task.Delay(500); // Simulate API call

                StatusMessage = "¡Gracias por suscribirse! Recibirá las últimas noticias y promociones en su correo electrónico.";
                
                // Clear the email field after successful subscription
                Email = string.Empty;
                
                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "Hubo un error al procesar su suscripción. Por favor intente nuevamente.";
                return Page();
            }
        }
    }
}