using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoClick.Pages
{
    public class TerminosCondicionesModel : PageModel
    {
        private readonly ILogger<TerminosCondicionesModel> _logger;

        public TerminosCondicionesModel(ILogger<TerminosCondicionesModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string? Section { get; set; }

        public string LastUpdated { get; set; } = "27 de septiembre de 2025";
        public string Version { get; set; } = "2.1";

        public IActionResult OnGet(string? section = null)
        {
            try
            {
                // Log page visit
                _logger.LogInformation("Términos y Condiciones page accessed at {Time}", DateTime.Now);

                // Set section if provided in query parameter
                if (!string.IsNullOrEmpty(section))
                {
                    Section = section.ToLower();
                    
                    // Validate section exists
                    var validSections = new[]
                    {
                        "aceptacion", "definiciones", "servicios", "registro", 
                        "uso-aceptable", "anuncios", "pagos", "propiedad", 
                        "privacidad", "limitacion", "terminacion", 
                        "modificaciones", "ley-aplicable", "contacto"
                    };

                    if (!validSections.Contains(Section))
                    {
                        Section = null;
                        _logger.LogWarning("Invalid section requested: {Section}", section);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Términos y Condiciones page");
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                // Handle any form submissions (newsletter, feedback, etc.)
                var email = Request.Form["email"].ToString();
                
                if (!string.IsNullOrEmpty(email))
                {
                    // Process newsletter subscription
                    _logger.LogInformation("Newsletter subscription from Terms page: {Email}", email);
                    TempData["NewsletterSuccess"] = "¡Gracias por suscribirte a nuestro newsletter!";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Terms page form submission");
                TempData["Error"] = "Ocurrió un error al procesar su solicitud. Por favor, inténtelo nuevamente.";
                return Page();
            }
        }

        public JsonResult OnGetSection(string sectionId)
        {
            try
            {
                // Return section information for AJAX requests
                var sectionInfo = GetSectionInfo(sectionId);
                
                if (sectionInfo != null)
                {
                    return new JsonResult(new { success = true, data = sectionInfo });
                }
                
                return new JsonResult(new { success = false, message = "Sección no encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving section info for {SectionId}", sectionId);
                return new JsonResult(new { success = false, message = "Error interno del servidor" });
            }
        }

        private object? GetSectionInfo(string sectionId)
        {
            var sections = new Dictionary<string, object>
            {
                ["aceptacion"] = new
                {
                    title = "Aceptación de Términos",
                    summary = "Condiciones para el uso de la plataforma AutoClick.cr",
                    lastModified = "27 de septiembre de 2025"
                },
                ["definiciones"] = new
                {
                    title = "Definiciones",
                    summary = "Términos y definiciones utilizados en estos términos y condiciones",
                    lastModified = "27 de septiembre de 2025"
                },
                ["servicios"] = new
                {
                    title = "Descripción de Servicios",
                    summary = "Servicios ofrecidos por AutoClick.cr a los usuarios",
                    lastModified = "27 de septiembre de 2025"
                },
                ["registro"] = new
                {
                    title = "Registro y Cuenta",
                    summary = "Requisitos y responsabilidades para crear y mantener una cuenta",
                    lastModified = "27 de septiembre de 2025"
                },
                ["uso-aceptable"] = new
                {
                    title = "Uso Aceptable",
                    summary = "Políticas sobre el uso apropiado de la plataforma",
                    lastModified = "27 de septiembre de 2025"
                },
                ["anuncios"] = new
                {
                    title = "Publicación de Anuncios",
                    summary = "Normas para la creación y gestión de anuncios de vehículos",
                    lastModified = "27 de septiembre de 2025"
                },
                ["pagos"] = new
                {
                    title = "Pagos y Transacciones",
                    summary = "Políticas relacionadas con pagos y transacciones comerciales",
                    lastModified = "27 de septiembre de 2025"
                },
                ["propiedad"] = new
                {
                    title = "Propiedad Intelectual",
                    summary = "Derechos de propiedad intelectual y licencias de uso",
                    lastModified = "27 de septiembre de 2025"
                },
                ["privacidad"] = new
                {
                    title = "Protección de Datos",
                    summary = "Políticas de privacidad y protección de datos personales",
                    lastModified = "27 de septiembre de 2025"
                },
                ["limitacion"] = new
                {
                    title = "Limitación de Responsabilidad",
                    summary = "Limitaciones de responsabilidad y exenciones de garantía",
                    lastModified = "27 de septiembre de 2025"
                },
                ["terminacion"] = new
                {
                    title = "Terminación",
                    summary = "Condiciones para la terminación de cuentas y servicios",
                    lastModified = "27 de septiembre de 2025"
                },
                ["modificaciones"] = new
                {
                    title = "Modificaciones",
                    summary = "Políticas sobre modificaciones a estos términos",
                    lastModified = "27 de septiembre de 2025"
                },
                ["ley-aplicable"] = new
                {
                    title = "Ley Aplicable",
                    summary = "Jurisdicción y ley aplicable para resolver disputas",
                    lastModified = "27 de septiembre de 2025"
                },
                ["contacto"] = new
                {
                    title = "Información de Contacto",
                    summary = "Datos de contacto para consultas legales y administrativas",
                    lastModified = "27 de septiembre de 2025"
                }
            };

            return sections.ContainsKey(sectionId) ? sections[sectionId] : null;
        }

        public string GetEstimatedReadingTime()
        {
            // Estimated reading time based on content length
            // Average reading speed: 200-250 words per minute
            var estimatedMinutes = 12; // Approximate for the full terms document
            return $"{estimatedMinutes} minutos";
        }

        public bool IsRecentlyUpdated()
        {
            // Check if terms were updated in the last 30 days
            var lastUpdateDate = new DateTime(2025, 9, 27);
            return (DateTime.Now - lastUpdateDate).TotalDays <= 30;
        }

        public string GetSectionAnchor(string sectionTitle)
        {
            // Convert section title to anchor format
            return sectionTitle.ToLower()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ñ", "n");
        }
    }
}