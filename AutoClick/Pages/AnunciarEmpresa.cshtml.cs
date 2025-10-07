using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class AnunciarEmpresaModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [StringLength(100, ErrorMessage = "El nombre de la empresa no puede exceder 100 caracteres")]
        public string NombreEmpresa { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El nombre del representante legal es requerido")]
        [StringLength(100, ErrorMessage = "El nombre del representante no puede exceder 100 caracteres")]
        public string RepresentanteLegal { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "La industria es requerida")]
        public string Industria { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [StringLength(150, ErrorMessage = "El correo electrónico no puede exceder 150 caracteres")]
        public string CorreoElectronico { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El teléfono es requerido")]
        [RegularExpression(@"^\+?[\d\s\-\(\)]{8,20}$", ErrorMessage = "Ingrese un número de teléfono válido")]
        public string Telefono { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "La descripción de la empresa es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string DescripcionEmpresa { get; set; } = "";

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public void OnGet()
        {
            // Initialize the page
            ErrorMessage = "";
            SuccessMessage = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Clear previous messages
                ErrorMessage = "";
                SuccessMessage = "";

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    ErrorMessage = string.Join(". ", errors);
                    return Page();
                }

                // Additional business validation
                if (!IsValidIndustria(Industria))
                {
                    ErrorMessage = "Por favor, seleccione una industria válida";
                    return Page();
                }

                // Validate email format (additional check)
                if (!IsValidEmail(CorreoElectronico))
                {
                    ErrorMessage = "Por favor, ingrese un correo electrónico válido";
                    return Page();
                }

                // Validate phone number (Costa Rica format)
                if (!IsValidCostaRicaPhone(Telefono))
                {
                    ErrorMessage = "Por favor, ingrese un número de teléfono válido de Costa Rica";
                    return Page();
                }

                // Process the business inquiry
                bool isProcessed = await ProcessBusinessInquiryAsync();

                if (isProcessed)
                {
                    SuccessMessage = "¡Gracias por su interés! Hemos recibido su solicitud y nos pondremos en contacto con usted pronto para discutir las oportunidades publicitarias en AutoClick.cr.";
                    
                    // Clear form data after successful submission
                    ClearFormData();
                    
                    // Store success message in TempData so it persists after redirect
                    TempData["SuccessMessage"] = SuccessMessage;
                    
                    return RedirectToPage();
                }
                else
                {
                    ErrorMessage = "Ocurrió un error al procesar su solicitud. Por favor, intente nuevamente o contáctenos directamente.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the error in a real application
                System.Diagnostics.Debug.WriteLine($"Business inquiry error: {ex.Message}");
                ErrorMessage = "Ocurrió un error interno. Por favor, intente nuevamente más tarde.";
                return Page();
            }
        }

        private bool IsValidIndustria(string industria)
        {
            var validIndustrias = new List<string>
            {
                "Automotriz", "Seguros", "Financiero", "Detailing",
                "Repuestos", "Concesionarios", "Talleres", "Otros"
            };
            
            return validIndustrias.Contains(industria);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidCostaRicaPhone(string phone)
        {
            // Remove common formatting characters
            string cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            
            // Costa Rica phone patterns:
            // +506 XXXX XXXX (international)
            // 506 XXXX XXXX (country code)
            // XXXX XXXX (local, 8 digits)
            
            if (cleanPhone.StartsWith("+506"))
            {
                cleanPhone = cleanPhone.Substring(4);
            }
            else if (cleanPhone.StartsWith("506"))
            {
                cleanPhone = cleanPhone.Substring(3);
            }
            
            // Should be 8 digits for Costa Rica
            return cleanPhone.Length == 8 && cleanPhone.All(char.IsDigit);
        }

        private async Task<bool> ProcessBusinessInquiryAsync()
        {
            try
            {
                // TODO: Implement actual business logic here
                // This could include:
                // - Saving to database
                // - Sending notification emails
                // - Creating a lead in CRM system
                // - Integrating with external services

                // Simulate async processing
                await Task.Delay(1000);
                
                // Log the inquiry for now
                System.Diagnostics.Debug.WriteLine($"New business inquiry received:");
                System.Diagnostics.Debug.WriteLine($"Company: {NombreEmpresa}");
                System.Diagnostics.Debug.WriteLine($"Contact: {RepresentanteLegal}");
                System.Diagnostics.Debug.WriteLine($"Industry: {Industria}");
                System.Diagnostics.Debug.WriteLine($"Email: {CorreoElectronico}");
                System.Diagnostics.Debug.WriteLine($"Phone: {Telefono}");
                System.Diagnostics.Debug.WriteLine($"Description: {DescripcionEmpresa}");
                
                // In a real application, you would:
                // 1. Save to database
                // 2. Send confirmation email to the business
                // 3. Send notification email to AutoClick.cr team
                // 4. Create a follow-up task/reminder
                // 5. Log the activity for analytics

                return true; // Simulate successful processing
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing business inquiry: {ex.Message}");
                return false;
            }
        }

        private void ClearFormData()
        {
            NombreEmpresa = "";
            RepresentanteLegal = "";
            Industria = "";
            CorreoElectronico = "";
            Telefono = "";
            DescripcionEmpresa = "";
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // Check if there's a success message in TempData
            if (TempData.ContainsKey("SuccessMessage"))
            {
                SuccessMessage = TempData["SuccessMessage"]?.ToString() ?? "";
            }
            
            base.OnPageHandlerExecuting(context);
        }
    }
}