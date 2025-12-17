using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResetPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Confirma la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool TokenIsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Token))
            {
                ErrorMessage = "Token de recuperación no proporcionado";
                TokenIsValid = false;
                return Page();
            }

            // Validar el token
            var resetToken = await _context.PasswordResetTokens
                .Where(t => t.Token == Token && !t.IsUsed)
                .FirstOrDefaultAsync();

            if (resetToken == null)
            {
                ErrorMessage = "El enlace de recuperación no es válido o ya fue utilizado";
                TokenIsValid = false;
                return Page();
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                ErrorMessage = "El enlace de recuperación ha expirado. Por favor, solicita uno nuevo";
                TokenIsValid = false;
                return Page();
            }

            TokenIsValid = true;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Token))
            {
                ErrorMessage = "Token de recuperación no proporcionado";
                TokenIsValid = false;
                return Page();
            }

            // Validar ModelState
            if (!ModelState.IsValid)
            {
                TokenIsValid = true;
                return Page();
            }

            // Validar requisitos de contraseña
            if (NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe tener al menos 8 caracteres");
                TokenIsValid = true;
                return Page();
            }

            if (!NewPassword.Any(char.IsUpper))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos una letra mayúscula");
                TokenIsValid = true;
                return Page();
            }

            if (!NewPassword.Any(char.IsLower))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos una letra minúscula");
                TokenIsValid = true;
                return Page();
            }

            if (!NewPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe contener al menos un número");
                TokenIsValid = true;
                return Page();
            }

            try
            {
                // Buscar el token
                var resetToken = await _context.PasswordResetTokens
                    .Where(t => t.Token == Token && !t.IsUsed)
                    .FirstOrDefaultAsync();

                if (resetToken == null)
                {
                    ErrorMessage = "El enlace de recuperación no es válido o ya fue utilizado";
                    TokenIsValid = false;
                    return Page();
                }

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    ErrorMessage = "El enlace de recuperación ha expirado. Por favor, solicita uno nuevo";
                    TokenIsValid = false;
                    return Page();
                }

                // Buscar el usuario
                var usuario = await _context.Usuarios.FindAsync(resetToken.Email);

                if (usuario == null)
                {
                    ErrorMessage = "Usuario no encontrado";
                    TokenIsValid = false;
                    return Page();
                }

                // Actualizar la contraseña
                usuario.Contrasena = PasswordHelper.HashPassword(NewPassword);
                _context.Entry(usuario).State = EntityState.Modified;

                // Marcar el token como usado
                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;
                _context.Entry(resetToken).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                // Mostrar mensaje de éxito
                SuccessMessage = "Tu contraseña ha sido restablecida exitosamente. Ya puedes iniciar sesión con tu nueva contraseña.";
                TokenIsValid = false;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ha ocurrido un error al restablecer tu contraseña: {ex.Message}";
                TokenIsValid = true;
                return Page();
            }
        }
    }
}
