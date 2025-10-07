using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoClick.Pages
{
    [Authorize]
    public class ConfiguracionCuentaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionCuentaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        public string Email { get; set; } = "usuario@autoclick.cr";

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        public string NewEmail { get; set; } = string.Empty;

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        [Compare("NewEmail", ErrorMessage = "Los correos electrónicos no coinciden")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        public string CurrentPasswordForEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Confirma la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        public string ForgotPasswordEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es obligatoria para eliminar la cuenta")]
        public string DeleteConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debes confirmar que entiendes que esta acción es irreversible")]
        public bool ConfirmDeletion { get; set; }

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        public string NewsletterEmail { get; set; } = string.Empty;

        public string MaskedEmail { get; private set; } = string.Empty;

        public void OnGet()
        {
            LoadCurrentUser();
        }

        private void LoadCurrentUser()
        {
            // Obtener el email del usuario actualmente logueado (o simulado)
            var currentUserEmail = GetCurrentUserEmail();
            
            if (!string.IsNullOrEmpty(currentUserEmail))
            {
                // Verificar que el usuario existe en la base de datos
                var usuario = _context.Usuarios.Find(currentUserEmail);
                if (usuario != null)
                {
                    Email = usuario.Email;
                }
                else
                {
                    // El usuario no existe en la BD, usar el email simulado
                    Email = currentUserEmail;
                    
                    // Opcional: Crear el usuario para que las funciones trabajen correctamente
                    try 
                    {
                        var nuevoUsuario = new Usuario
                        {
                            Email = currentUserEmail,
                            Nombre = "Usuario",
                            Apellidos = "Administrador", 
                            NumeroTelefono = "8888-8888",
                            Contrasena = "temporal123" // En producción esto estaría hasheado
                        };
                        
                        _context.Usuarios.Add(nuevoUsuario);
                        _context.SaveChanges();
                    }
                    catch
                    {
                        // Si hay error al crear, continuar con el email
                    }
                }
            }
            else
            {
                // Fallback
                Email = "admin@gmail.com";
            }
            
            MaskedEmail = MaskEmail(Email);
        }

        private string GetCurrentUserEmail()
        {
            // TEMPORAL: Simular usuario logueado hasta que se implemente el sistema de autenticación
            // TODO: Reemplazar con el sistema de autenticación real
            
            // Verificar si hay un sistema de autenticación configurado
            if (User.Identity?.IsAuthenticated == true)
            {
                // Intentar obtener el email de diferentes claims comunes
                return User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                    ?? User.FindFirst("email")?.Value 
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? "admin@gmail.com"; // Fallback
            }
            
            // Simulación temporal - reemplazar cuando tengas login real
            return "admin@gmail.com";
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                // Verificar contraseña actual (simulado)
                if (!VerifyCurrentPassword(CurrentPasswordForEmail))
                {
                    ModelState.AddModelError("CurrentPasswordForEmail", "La contraseña actual no es correcta");
                    OnGet();
                    return Page();
                }

                // Verificar que el nuevo email no esté en uso (simulado)
                if (await IsEmailInUse(NewEmail))
                {
                    ModelState.AddModelError("NewEmail", "Este correo electrónico ya está en uso");
                    OnGet();
                    return Page();
                }

                // Simular cambio de email
                await Task.Delay(500);
                Email = NewEmail;
                MaskedEmail = MaskEmail(Email);

                TempData["SuccessMessage"] = "Tu correo electrónico ha sido actualizado exitosamente";
                return RedirectToPage();
            }
            catch (Exception _)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al cambiar el correo electrónico. Intenta de nuevo.");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                // Verificar contraseña actual (simulado)
                if (!VerifyCurrentPassword(CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "La contraseña actual no es correcta");
                    OnGet();
                    return Page();
                }

                // Simular cambio de contraseña
                await Task.Delay(500);

                TempData["SuccessMessage"] = "Tu contraseña ha sido actualizada exitosamente";
                return RedirectToPage();
            }
            catch (Exception _)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al cambiar la contraseña. Intenta de nuevo.");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostForgotPasswordAsync()
        {
            if (string.IsNullOrEmpty(ForgotPasswordEmail) || ModelState["ForgotPasswordEmail"]?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                ModelState.AddModelError("ForgotPasswordEmail", "Ingresa un correo electrónico válido");
                OnGet();
                return Page();
            }

            try
            {
                // Simular envío de email de recuperación
                await Task.Delay(1000);

                TempData["SuccessMessage"] = "Se ha enviado un enlace de recuperación a tu correo electrónico";
                return RedirectToPage();
            }
            catch (Exception _)
            {
                ModelState.AddModelError("ForgotPasswordEmail", "Ha ocurrido un error al enviar el enlace de recuperación. Intenta de nuevo.");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                // Verificar contraseña actual (simulado)
                if (!VerifyCurrentPassword(DeleteConfirmPassword))
                {
                    ModelState.AddModelError("DeleteConfirmPassword", "La contraseña no es correcta");
                    OnGet();
                    return Page();
                }

                // Simular eliminación de cuenta
                await Task.Delay(1000);

                // Redireccionar a página de despedida o login
                TempData["AccountDeleted"] = "Tu cuenta ha sido eliminada exitosamente";
                return RedirectToPage("/Index");
            }
            catch (Exception _)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al eliminar la cuenta. Intenta de nuevo.");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostNewsletterAsync()
        {
            if (string.IsNullOrEmpty(NewsletterEmail) || ModelState["NewsletterEmail"]?.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
            {
                OnGet();
                return Page();
            }

            try
            {
                // Simular suscripción al newsletter
                await Task.Delay(500);

                TempData["NewsletterSuccess"] = "¡Te has suscrito exitosamente al newsletter!";
                return RedirectToPage();
            }
            catch (Exception _)
            {
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAccountAsync()
        {
            // Este método maneja actualizaciones generales de la cuenta
            try
            {
                await Task.Delay(500);
                TempData["SuccessMessage"] = "La información de tu cuenta ha sido actualizada";
                return RedirectToPage();
            }
            catch (Exception _)
            {
                ModelState.AddModelError("", "Ha ocurrido un error al actualizar la información. Intenta de nuevo.");
                OnGet();
                return Page();
            }
        }

        private bool VerifyCurrentPassword(string password)
        {
            // Simular verificación de contraseña
            // En una implementación real, aquí se verificaría contra la base de datos
            return !string.IsNullOrEmpty(password) && password.Length >= 6;
        }

        private async Task<bool> IsEmailInUse(string email)
        {
            // Simular verificación de email en uso
            // En una implementación real, aquí se consultaría la base de datos
            await Task.Delay(100);
            
            // Simular algunos emails ya en uso
            var usedEmails = new[] { "admin@autoclick.cr", "test@autoclick.cr", "demo@autoclick.cr" };
            return usedEmails.Contains(email.ToLower());
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