using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class AuthModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthModel(ApplicationDbContext context, IAuthService authService, IEmailService emailService)
        {
            _context = context;
            _authService = authService;
            _emailService = emailService;
        }
        public string TipoUsuario { get; set; } = "";

        [BindProperty]
        public string LoginEmail { get; set; } = "";

        [BindProperty]
        public string LoginPassword { get; set; } = "";

        [BindProperty]
        public bool RememberMe { get; set; } = false;

        [BindProperty]
        public string ForgotPasswordEmail { get; set; } = "";

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGet()
        {
            // Manejar logout si viene el parámetro
            if (Request.Query.ContainsKey("logout"))
            {
                // Limpiar todas las sesiones
                HttpContext.Session.Clear();
                SuccessMessage = "Sesión cerrada exitosamente";
                return Page();
            }

            // Guardar returnUrl en sesión si viene como parámetro
            ReturnUrl = Request.Query["returnUrl"];
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                HttpContext.Session.SetString("ReturnUrl", ReturnUrl);
            }

            // Initialize the page
            ErrorMessage = "";
            SuccessMessage = "";
            return Page();
        }

        public IActionResult OnPostRegister()
        {
            try
            {
                // Manually bind TipoUsuario from form
                TipoUsuario = Request.Form["TipoUsuario"].ToString() ?? "";
                
                if (string.IsNullOrEmpty(TipoUsuario))
                {
                    ErrorMessage = "Debe seleccionar un tipo de usuario";
                    return Page();
                }

                // Validate user type
                if (TipoUsuario != "Individuo" && TipoUsuario != "Agencia")
                {
                    ErrorMessage = "Tipo de usuario inválido";
                    return Page();
                }

                // Store the selected user type in session for the next step
                HttpContext.Session.SetString("TipoUsuario", TipoUsuario);

                // Redirect to appropriate registration page based on user type
                if (TipoUsuario == "Agencia")
                {
                    return RedirectToPage("/RegistroAgencia");
                }
                else
                {
                    return RedirectToPage("/RegistroIndividuo");
                }
            }
            catch (Exception ex)
            {
                // Log the error in a real application

                ErrorMessage = "Ocurrió un error al procesar su solicitud. Intente nuevamente.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            try
            {
                // Clear previous error messages
                ErrorMessage = "";

                // Validate login fields manually
                bool isValid = true;
                
                if (string.IsNullOrWhiteSpace(LoginEmail))
                {
                    ErrorMessage = "El correo electrónico es requerido";
                    isValid = false;
                }
                else if (!IsValidEmail(LoginEmail))
                {
                    ErrorMessage = "Por favor, ingrese un correo electrónico válido";
                    isValid = false;
                }
                
                if (string.IsNullOrWhiteSpace(LoginPassword))
                {
                    ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "La contraseña es requerida" : ErrorMessage + ". La contraseña es requerida";
                    isValid = false;
                }
                else if (LoginPassword.Length < 6)
                {
                    ErrorMessage = string.IsNullOrEmpty(ErrorMessage) ? "La contraseña debe tener al menos 6 caracteres" : ErrorMessage + ". La contraseña debe tener al menos 6 caracteres";
                    isValid = false;
                }
                
                if (!isValid)
                {
                    return Page();
                }

                // Use AuthService for authentication
                var result = await _authService.LoginAsync(LoginEmail, LoginPassword);
                
                if (result.Success)
                {
                    // Verificar returnUrl del formulario (campo oculto) o de la sesión
                    string? returnUrl = ReturnUrl ?? HttpContext.Session.GetString("ReturnUrl");
                    HttpContext.Session.Remove("ReturnUrl"); // Limpiar después de usar
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = result.Message;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the error in a real application

                ErrorMessage = "Ocurrió un error al iniciar sesión. Intente nuevamente.";
                return Page();
            }
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



        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await _authService.LogoutAsync();
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            try
            {
                // Validar que se proporcionó un email
                if (string.IsNullOrWhiteSpace(ForgotPasswordEmail))
                {
                    return new JsonResult(new { 
                        success = false, 
                        message = "Por favor, ingrese su correo electrónico" 
                    });
                }

                // Validar formato del email
                if (!IsValidEmail(ForgotPasswordEmail))
                {
                    return new JsonResult(new { 
                        success = false, 
                        message = "Por favor, ingrese un correo electrónico válido" 
                    });
                }

                // Normalizar email
                var email = ForgotPasswordEmail.Trim().ToLower();

                // Buscar usuario en la base de datos
                var usuario = await _context.Usuarios.FindAsync(email);

                // IMPORTANTE: No revelar si el usuario existe o no (seguridad)
                // Siempre devolver el mismo mensaje de éxito
                if (usuario == null)
                {
                    // Simular tiempo de procesamiento para evitar timing attacks
                    await Task.Delay(500);
                    
                    return new JsonResult(new { 
                        success = true, 
                        message = "Si el correo está registrado, recibirá instrucciones para recuperar su contraseña",
                        email = MaskEmail(email)
                    });
                }

                // Verificar si existe un token reciente (dentro de 15 minutos)
                var recentToken = await _context.PasswordResetTokens
                    .Where(t => t.Email == email && 
                                !t.IsUsed && 
                                t.LastEmailSentAt.HasValue &&
                                t.LastEmailSentAt.Value.AddMinutes(15) > DateTime.UtcNow)
                    .OrderByDescending(t => t.LastEmailSentAt)
                    .FirstOrDefaultAsync();

                if (recentToken != null)
                {
                    var timeRemaining = (int)(recentToken.LastEmailSentAt.Value.AddMinutes(15) - DateTime.UtcNow).TotalSeconds;
                    return new JsonResult(new { 
                        success = false, 
                        message = "Ya se envió un correo recientemente. Por favor, espere 15 minutos antes de solicitar otro",
                        secondsRemaining = timeRemaining,
                        email = MaskEmail(email)
                    });
                }

                // Generar nuevo token
                var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"); // Token largo y único
                var resetToken = new PasswordResetToken
                {
                    Email = email,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsUsed = false,
                    LastEmailSentAt = DateTime.UtcNow
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Enviar email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(email, token);

                if (emailSent)
                {
                    return new JsonResult(new { 
                        success = true, 
                        message = "Si el correo está registrado, recibirá instrucciones para recuperar su contraseña",
                        email = MaskEmail(email)
                    });
                }
                else
                {
                    // No revelar que hubo un error específico con el envío
                    return new JsonResult(new { 
                        success = true, 
                        message = "Si el correo está registrado, recibirá instrucciones para recuperar su contraseña",
                        email = MaskEmail(email)
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error pero no revelar detalles al usuario
                Console.WriteLine($"Error en recuperación de contraseña: {ex.Message}");
                
                return new JsonResult(new { 
                    success = false, 
                    message = "Ha ocurrido un error. Por favor, intente nuevamente más tarde" 
                });
            }
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var localPart = parts[0];
            var domainPart = parts[1];

            // Mostrar solo los primeros y últimos caracteres del local part
            if (localPart.Length <= 3)
            {
                return new string('*', localPart.Length) + "@" + domainPart;
            }

            var maskedLocal = localPart.Substring(0, 2) + 
                             new string('*', Math.Max(0, localPart.Length - 4)) + 
                             localPart.Substring(Math.Max(2, localPart.Length - 2));

            return maskedLocal + "@" + domainPart;
        }
    }
}