using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace AutoClick.Pages
{
    public class PoliticaPrivacidadModel : PageModel
    {
        private readonly ILogger<PoliticaPrivacidadModel> _logger;

        public PoliticaPrivacidadModel(ILogger<PoliticaPrivacidadModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string? Section { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        public string LastUpdated { get; set; } = "19 de agosto de 2025";
        public string Version { get; set; } = "2.1";
        public string LawReference { get; set; } = "Ley N.º 8968";

        public IActionResult OnGet(string? section = null)
        {
            try
            {
                // Log page visit
                _logger.LogInformation("Política de Privacidad page accessed at {Time}", DateTime.Now);

                // Set section if provided in query parameter
                if (!string.IsNullOrEmpty(section))
                {
                    Section = section.ToLower();
                    
                    // Validate section exists
                    var validSections = new[]
                    {
                        "introduccion", "responsable", "finalidad", "datos-recopilados", 
                        "consentimiento", "confidencialidad", "derechos-titular", 
                        "seguridad", "cookies", "modificaciones", "legislacion", "contacto"
                    };

                    if (!validSections.Contains(Section))
                    {
                        Section = null;
                        _logger.LogWarning("Invalid privacy policy section requested: {Section}", section);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Política de Privacidad page");
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnPostNewsletter()
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    TempData["NewsletterError"] = "Por favor, ingrese un correo electrónico válido.";
                    return Page();
                }

                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                if (!emailRegex.IsMatch(Email))
                {
                    TempData["NewsletterError"] = "Por favor, ingrese un formato de correo electrónico válido.";
                    return Page();
                }

                // Process newsletter subscription
                _logger.LogInformation("Newsletter subscription from Privacy Policy page: {Email}", Email);
                
                // Here you would typically save to database or send to email service
                // For now, we'll just log and show success message
                
                TempData["NewsletterSuccess"] = "¡Gracias por suscribirte! Recibirás nuestras actualizaciones de privacidad y noticias importantes.";
                
                // Clear the email field after successful subscription
                Email = string.Empty;
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing newsletter subscription from Privacy Policy page");
                TempData["NewsletterError"] = "Ocurrió un error al procesar su suscripción. Por favor, inténtelo nuevamente.";
                return Page();
            }
        }

        public IActionResult OnPostDataRequest(string requestType)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    TempData["DataRequestError"] = "Por favor, proporcione su correo electrónico para procesar la solicitud.";
                    return Page();
                }

                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                if (!emailRegex.IsMatch(Email))
                {
                    TempData["DataRequestError"] = "Por favor, ingrese un formato de correo electrónico válido.";
                    return Page();
                }

                // Log the data request
                _logger.LogInformation("Data request of type {RequestType} from {Email} at {Time}", 
                    requestType, Email, DateTime.Now);

                string successMessage = requestType switch
                {
                    "access" => "Su solicitud de acceso a datos ha sido recibida. Le responderemos en un plazo de 5 días hábiles.",
                    "rectification" => "Su solicitud de rectificación ha sido recibida. Procesaremos los cambios solicitados.",
                    "deletion" => "Su solicitud de eliminación de datos ha sido recibida. Le confirmaremos el proceso por correo electrónico.",
                    "portability" => "Su solicitud de portabilidad de datos ha sido recibida. Preparemos sus datos para la transferencia.",
                    "opposition" => "Su oposición al procesamiento ha sido registrada. Revisaremos y suspenderemos el procesamiento correspondiente.",
                    _ => "Su solicitud ha sido recibida y será procesada según nuestra política de privacidad."
                };

                TempData["DataRequestSuccess"] = successMessage;

                // Here you would typically:
                // 1. Create a data request record in the database
                // 2. Send confirmation email to the user
                // 3. Notify the data protection team
                // 4. Start the appropriate process based on request type

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data request of type {RequestType}", requestType);
                TempData["DataRequestError"] = "Ocurrió un error al procesar su solicitud. Por favor, contacte directamente a g.montero@AutoClick.cr";
                return Page();
            }
        }

        public JsonResult OnGetSectionInfo(string sectionId)
        {
            try
            {
                var sectionInfo = GetSectionInformation(sectionId);
                
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

        private object? GetSectionInformation(string sectionId)
        {
            var sections = new Dictionary<string, object>
            {
                ["introduccion"] = new
                {
                    title = "Introducción",
                    summary = "Compromiso de AutoClick.cr con la protección de datos personales",
                    keyPoints = new[] { "Ley 8968", "Normativa costarricense", "Protección de privacidad" }
                },
                ["responsable"] = new
                {
                    title = "Responsable del Tratamiento",
                    summary = "Información de contacto del responsable de datos",
                    keyPoints = new[] { "AutoClick.cr", "Cédula 208080611", "g.montero@AutoClick.cr" }
                },
                ["finalidad"] = new
                {
                    title = "Finalidad de la Recolección",
                    summary = "Propósitos para los cuales se recopilan los datos personales",
                    keyPoints = new[] { "Gestión de cuentas", "Publicación de anuncios", "Comunicación entre usuarios" }
                },
                ["datos-recopilados"] = new
                {
                    title = "Datos que Recopilamos",
                    summary = "Tipos de información personal que solicitamos",
                    keyPoints = new[] { "Información personal", "Datos de vehículos", "Información técnica" }
                },
                ["consentimiento"] = new
                {
                    title = "Consentimiento del Usuario",
                    summary = "Cómo se obtiene y puede revocarse el consentimiento",
                    keyPoints = new[] { "Consentimiento libre", "Derecho de revocación", "Artículo 5 Ley 8968" }
                },
                ["confidencialidad"] = new
                {
                    title = "Confidencialidad y Transferencia",
                    summary = "Política de protección y compartición de datos",
                    keyPoints = new[] { "No venta de datos", "Excepciones legales", "Terceros autorizados" }
                },
                ["derechos-titular"] = new
                {
                    title = "Derechos del Titular",
                    summary = "Derechos que tienen los usuarios sobre sus datos",
                    keyPoints = new[] { "Acceso", "Rectificación", "Cancelación", "Oposición" }
                },
                ["seguridad"] = new
                {
                    title = "Medidas de Seguridad",
                    summary = "Protecciones implementadas para salvaguardar los datos",
                    keyPoints = new[] { "Medidas administrativas", "Protección técnica", "Seguridad física" }
                },
                ["cookies"] = new
                {
                    title = "Cookies y Tecnologías",
                    summary = "Uso de cookies y tecnologías de seguimiento",
                    keyPoints = new[] { "Análisis de navegación", "Mejora de experiencia", "Control del usuario" }
                },
                ["modificaciones"] = new
                {
                    title = "Modificaciones a la Política",
                    summary = "Cómo se comunican los cambios en la política",
                    keyPoints = new[] { "Notificación por email", "Aviso en sitio web", "Vigencia inmediata" }
                },
                ["legislacion"] = new
                {
                    title = "Legislación Aplicable",
                    summary = "Marco legal y jurisdicción aplicable",
                    keyPoints = new[] { "Ley 8968", "Legislación costarricense", "Tribunales de San José" }
                },
                ["contacto"] = new
                {
                    title = "Información de Contacto",
                    summary = "Canales para ejercer derechos y hacer consultas",
                    keyPoints = new[] { "g.montero@AutoClick.cr", "5 días hábiles", "Oficinas en San José" }
                }
            };

            return sections.ContainsKey(sectionId) ? sections[sectionId] : null;
        }

        public string GetEstimatedReadingTime()
        {
            // Estimated reading time based on content length
            // Average reading speed: 200-250 words per minute
            var estimatedMinutes = 15; // Approximate for the full privacy policy document
            return $"{estimatedMinutes} minutos";
        }

        public bool IsRecentlyUpdated()
        {
            // Check if policy was updated in the last 60 days
            var lastUpdateDate = new DateTime(2025, 8, 19);
            return (DateTime.Now - lastUpdateDate).TotalDays <= 60;
        }

        public List<string> GetDataCategories()
        {
            return new List<string>
            {
                "Información de identificación personal",
                "Datos de contacto",
                "Información de vehículos",
                "Datos de navegación y uso",
                "Información de pago",
                "Preferencias y configuraciones",
                "Comunicaciones y correspondencia"
            };
        }

        public List<string> GetUserRights()
        {
            return new List<string>
            {
                "Derecho de acceso a sus datos personales",
                "Derecho de rectificación de datos inexactos",
                "Derecho de cancelación o eliminación",
                "Derecho de oposición al procesamiento",
                "Derecho a la portabilidad de datos",
                "Derecho a limitar el procesamiento",
                "Derecho de revocación del consentimiento"
            };
        }

        public string GetLegalBasis(string dataType)
        {
            return dataType switch
            {
                "account" => "Consentimiento del usuario y ejecución de contrato",
                "marketing" => "Consentimiento explícito del usuario",
                "legal" => "Cumplimiento de obligaciones legales",
                "security" => "Intereses legítimos de seguridad",
                _ => "Consentimiento del usuario"
            };
        }
    }
}