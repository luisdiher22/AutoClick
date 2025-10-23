using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;

namespace AutoClick.Pages
{
    public class AnunciarEmpresaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AnunciarEmpresaModel> _logger;

        public AnunciarEmpresaModel(
            ApplicationDbContext context, 
            IEmailService emailService,
            ILogger<AnunciarEmpresaModel> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }
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
                _logger.LogInformation("Procesando nueva solicitud de empresa");

                // 1. Crear y guardar la solicitud en la base de datos
                var solicitud = new SolicitudEmpresa
                {
                    NombreEmpresa = NombreEmpresa,
                    RepresentanteLegal = RepresentanteLegal,
                    Industria = Industria,
                    CorreoElectronico = CorreoElectronico,
                    Telefono = Telefono,
                    DescripcionEmpresa = DescripcionEmpresa,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "Pendiente"
                };

                _context.SolicitudesEmpresa.Add(solicitud);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Solicitud guardada en BD con ID: {solicitud.Id}");

                // 2. Obtener correos de todos los administradores
                var correosAdmins = await _context.Usuarios
                    .Where(u => u.EsAdministrador == true)
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!correosAdmins.Any())
                {
                    _logger.LogWarning("No se encontraron administradores en el sistema");
                    // Usar un correo por defecto si no hay admins
                    correosAdmins.Add("admin@autoclick.cr");
                }

                _logger.LogInformation($"Se encontraron {correosAdmins.Count} administrador(es)");

                // 3. Enviar email a los administradores
                bool emailEnviado = await _emailService.EnviarNotificacionSolicitudEmpresaAsync(solicitud, correosAdmins);

                if (emailEnviado)
                {
                    _logger.LogInformation("Email de notificación enviado exitosamente");
                }
                else
                {
                    _logger.LogWarning("No se pudo enviar el email de notificación");
                }

                return true; // La solicitud se guardó correctamente
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar solicitud de empresa: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
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