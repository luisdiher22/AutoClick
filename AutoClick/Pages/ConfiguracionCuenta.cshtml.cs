using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace AutoClick.Pages
{
    [Authorize]
    public class ConfiguracionCuentaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly Services.IEmailService _emailService;

        public ConfiguracionCuentaModel(ApplicationDbContext context, Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty]
        public string Email { get; set; } = "usuario@autoclick.cr";

        [BindProperty]
        public string CurrentEmailForChange { get; set; } = string.Empty;

        [BindProperty]
        public string CurrentPasswordForEmail { get; set; } = string.Empty;

        [BindProperty]
        public string NewEmail { get; set; } = string.Empty;

        [BindProperty]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ForgotPasswordEmail { get; set; } = string.Empty;

        [BindProperty]
        public string DeleteConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public bool ConfirmDeletion { get; set; }

        [BindProperty]
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
                            Contrasena = PasswordHelper.HashPassword("temporal123") // Contraseña hasheada
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
            // Limpiar ModelState de campos no relevantes para este formulario
            ModelState.Remove(nameof(Email));
            ModelState.Remove(nameof(CurrentPassword));
            ModelState.Remove(nameof(NewPassword));
            ModelState.Remove(nameof(ConfirmPassword));
            ModelState.Remove(nameof(ForgotPasswordEmail));
            ModelState.Remove(nameof(DeleteConfirmPassword));
            ModelState.Remove(nameof(ConfirmDeletion));
            ModelState.Remove(nameof(NewsletterEmail));
            
            // Validación manual de campos requeridos
            if (string.IsNullOrWhiteSpace(CurrentEmailForChange))
            {
                ModelState.AddModelError("CurrentEmailForChange", "El correo actual es obligatorio");
            }
            
            if (string.IsNullOrWhiteSpace(CurrentPasswordForEmail))
            {
                ModelState.AddModelError("CurrentPasswordForEmail", "La contraseña actual es obligatoria");
            }
            
            if (string.IsNullOrWhiteSpace(NewEmail))
            {
                ModelState.AddModelError("NewEmail", "El nuevo correo es obligatorio");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(NewEmail, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                ModelState.AddModelError("NewEmail", "El correo debe tener el formato @gmail.com");
            }
            
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                var currentUserEmail = GetCurrentUserEmail();
                
                // Verificar que el correo actual ingresado coincide con el correo actual del usuario
                if (CurrentEmailForChange.Trim().ToLower() != currentUserEmail.ToLower())
                {
                    ModelState.AddModelError("CurrentEmailForChange", "El correo actual no es correcto");
                    OnGet();
                    return Page();
                }

                // Buscar el usuario en la base de datos
                var usuario = await _context.Usuarios.FindAsync(currentUserEmail);
                
                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario no encontrado en el sistema");
                    OnGet();
                    return Page();
                }

                // Verificar contraseña actual usando el hash
                if (!PasswordHelper.VerifyPassword(CurrentPasswordForEmail, usuario.Contrasena))
                {
                    ModelState.AddModelError("CurrentPasswordForEmail", "La contraseña actual no es correcta");
                    OnGet();
                    return Page();
                }

                // Verificar que el nuevo email no esté en uso
                var emailExists = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == NewEmail.ToLower());
                if (emailExists)
                {
                    ModelState.AddModelError("NewEmail", "Este correo electrónico ya está en uso");
                    OnGet();
                    return Page();
                }

                // IMPORTANTE: Como Email es Primary Key y hay FKs que lo referencian,
                // usamos SQL raw para actualizar directamente con deshabilitar/habilitar constraints
                // Solo Autos y Favoritos tienen FK constraints (Mensajes y Reclamos no tienen FK)
                
                // Usar la estrategia de ejecución para manejar transacciones con reintentos
                var strategy = _context.Database.CreateExecutionStrategy();
                
                await strategy.ExecuteAsync(async () =>
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        // 1. Deshabilitar temporalmente las constraints de FK (solo Autos y Favoritos)
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Autos NOCHECK CONSTRAINT FK_Autos_Usuarios_EmailPropietario");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Favoritos NOCHECK CONSTRAINT FK_Favoritos_Usuarios_EmailUsuario");
                        
                        // 2. Actualizar todas las FK primero
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Autos SET EmailPropietario = {0} WHERE EmailPropietario = {1}", 
                            NewEmail, currentUserEmail);
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Favoritos SET EmailUsuario = {0} WHERE EmailUsuario = {1}", 
                            NewEmail, currentUserEmail);
                        
                        // Actualizar Mensajes (no tiene FK, solo columnas de texto)
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Mensajes SET EmailCliente = {0} WHERE EmailCliente = {1}", 
                            NewEmail, currentUserEmail);
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Mensajes SET EmailAdminRespuesta = {0} WHERE EmailAdminRespuesta = {1}", 
                            NewEmail, currentUserEmail);
                        
                        // Actualizar Reclamos (no tiene FK, solo columnas de texto)
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Reclamos SET EmailCliente = {0} WHERE EmailCliente = {1}", 
                            NewEmail, currentUserEmail);
                        
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Reclamos SET EmailAdminRespuesta = {0} WHERE EmailAdminRespuesta = {1}", 
                            NewEmail, currentUserEmail);
                        
                        // 3. Actualizar el Email del usuario (Primary Key)
                        await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE Usuarios SET Email = {0} WHERE Email = {1}", 
                            NewEmail, currentUserEmail);
                        
                        // 4. Reactivar las constraints de FK (solo Autos y Favoritos)
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Autos CHECK CONSTRAINT FK_Autos_Usuarios_EmailPropietario");
                        await _context.Database.ExecuteSqlRawAsync("ALTER TABLE Favoritos CHECK CONSTRAINT FK_Favoritos_Usuarios_EmailUsuario");
                        
                        // Commit de la transacción
                        await transaction.CommitAsync();
                    }
                });
                
                // Actualizar claims en la sesión actual con el nuevo email
                await UpdateSessionEmailAsync(NewEmail);
                
                TempData["SuccessMessage"] = "Tu correo electrónico ha sido actualizado exitosamente.";
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"Ha ocurrido un error al cambiar el correo electrónico: {ex.Message}");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            // Limpiar ModelState de campos no relevantes para este formulario
            ModelState.Remove(nameof(Email));
            ModelState.Remove(nameof(CurrentEmailForChange));
            ModelState.Remove(nameof(CurrentPasswordForEmail));
            ModelState.Remove(nameof(NewEmail));
            ModelState.Remove(nameof(ForgotPasswordEmail));
            ModelState.Remove(nameof(DeleteConfirmPassword));
            ModelState.Remove(nameof(ConfirmDeletion));
            ModelState.Remove(nameof(NewsletterEmail));
            
            // Validación manual de campos requeridos
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es obligatoria");
            }
            
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ModelState.AddModelError("NewPassword", "La nueva contraseña es obligatoria");
            }
            else if (NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe tener al menos 8 caracteres");
            }
            
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Confirma la nueva contraseña");
            }
            else if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
            }
            
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                var currentUserEmail = GetCurrentUserEmail();
                
                var usuario = await _context.Usuarios.FindAsync(currentUserEmail);
                
                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario no encontrado en el sistema");
                    OnGet();
                    return Page();
                }

                // Verificar contraseña actual usando el hash
                if (!PasswordHelper.VerifyPassword(CurrentPassword, usuario.Contrasena))
                {
                    ModelState.AddModelError("CurrentPassword", "La contraseña actual no es correcta");
                    OnGet();
                    return Page();
                }

                // Actualizar la contraseña en la base de datos (hasheada)
                usuario.Contrasena = PasswordHelper.HashPassword(NewPassword);
                
                // Marcar explícitamente como modificado
                _context.Entry(usuario).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tu contraseña ha sido actualizada exitosamente.";
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"Ha ocurrido un error al cambiar la contraseña: {ex.Message}");
                OnGet();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostForgotPasswordAutoAsync()
        {
            try
            {
                // Obtener el email del usuario actual
                var currentUserEmail = GetCurrentUserEmail();
                
                if (string.IsNullOrEmpty(currentUserEmail))
                {
                    return new JsonResult(new { success = false, message = "No se pudo obtener el correo del usuario" });
                }

                // Verificar que el correo existe en la base de datos
                var usuario = await _context.Usuarios.FindAsync(currentUserEmail);
                
                if (usuario == null)
                {
                    return new JsonResult(new { 
                        success = false, 
                        message = "Usuario no encontrado en el sistema"
                    });
                }

                // Verificar si existe un token reciente (dentro de 15 minutos)
                var recentToken = await _context.PasswordResetTokens
                    .Where(t => t.Email == currentUserEmail && 
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
                        message = "Debes esperar 15 minutos antes de solicitar otro correo",
                        secondsRemaining = timeRemaining,
                        email = MaskEmail(currentUserEmail),
                        userEmail = currentUserEmail
                    });
                }

                // Generar nuevo token
                var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"); // Token largo y unico
                var resetToken = new PasswordResetToken
                {
                    Email = currentUserEmail,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsUsed = false,
                    LastEmailSentAt = DateTime.UtcNow
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                // Enviar email
                var emailSent = await _emailService.SendPasswordResetEmailAsync(currentUserEmail, token);

                if (emailSent)
                {
                    return new JsonResult(new { 
                        success = true, 
                        message = "Correo enviado exitosamente",
                        email = MaskEmail(currentUserEmail),
                        userEmail = currentUserEmail,
                        tokenId = resetToken.Id
                    });
                }
                else
                {
                    return new JsonResult(new { 
                        success = false, 
                        message = "Error al enviar el correo. Intenta de nuevo mas tarde." 
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Ha ocurrido un error al procesar tu solicitud." 
                });
            }
        }

        public async Task<IActionResult> OnPostResendPasswordResetAsync([FromBody] ResendPasswordResetRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Email))
                {
                    return new JsonResult(new { success = false, message = "Email no proporcionado" });
                }

                // Verificar si puede reenviar (15 minutos de cooldown)
                var lastToken = await _context.PasswordResetTokens
                    .Where(t => t.Email == request.Email && 
                                !t.IsUsed && 
                                t.LastEmailSentAt.HasValue)
                    .OrderByDescending(t => t.LastEmailSentAt)
                    .FirstOrDefaultAsync();

                if (lastToken != null)
                {
                    var timeSinceLastEmail = DateTime.UtcNow - lastToken.LastEmailSentAt.Value;
                    if (timeSinceLastEmail.TotalMinutes < 15)
                    {
                        var secondsRemaining = (int)(900 - timeSinceLastEmail.TotalSeconds);
                        return new JsonResult(new { 
                            success = false, 
                            message = "Debes esperar antes de reenviar el correo",
                            secondsRemaining = secondsRemaining
                        });
                    }

                    // Actualizar el timestamp del último envío
                    lastToken.LastEmailSentAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Reenviar el email con el mismo token
                    var emailSent = await _emailService.SendPasswordResetEmailAsync(request.Email, lastToken.Token);
                    
                    if (emailSent)
                    {
                        return new JsonResult(new { 
                            success = true, 
                            message = "Correo reenviado exitosamente" 
                        });
                    }
                }

                return new JsonResult(new { 
                    success = false, 
                    message = "No se pudo reenviar el correo" 
                });
            }
            catch (Exception)
            {
                return new JsonResult(new { 
                    success = false, 
                    message = "Error al reenviar el correo" 
                });
            }
        }

        public class ResendPasswordResetRequest
        {
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            // Limpiar ModelState de campos no relevantes para este formulario
            ModelState.Remove(nameof(Email));
            ModelState.Remove(nameof(CurrentEmailForChange));
            ModelState.Remove(nameof(CurrentPasswordForEmail));
            ModelState.Remove(nameof(NewEmail));
            ModelState.Remove(nameof(CurrentPassword));
            ModelState.Remove(nameof(NewPassword));
            ModelState.Remove(nameof(ConfirmPassword));
            ModelState.Remove(nameof(ForgotPasswordEmail));
            ModelState.Remove(nameof(NewsletterEmail));
            
            // Validación manual de campos requeridos
            if (string.IsNullOrWhiteSpace(DeleteConfirmPassword))
            {
                ModelState.AddModelError("DeleteConfirmPassword", "La contraseña es obligatoria para eliminar la cuenta");
            }
            
            if (!ConfirmDeletion)
            {
                ModelState.AddModelError("ConfirmDeletion", "Debes confirmar que entiendes que esta acción es irreversible");
            }
            
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            try
            {
                var currentUserEmail = GetCurrentUserEmail();
                var usuario = await _context.Usuarios.FindAsync(currentUserEmail);
                
                if (usuario == null)
                {
                    ModelState.AddModelError("", "Usuario no encontrado en el sistema");
                    OnGet();
                    return Page();
                }

                // Verificar contraseña actual usando el hash
                if (!PasswordHelper.VerifyPassword(DeleteConfirmPassword, usuario.Contrasena))
                {
                    ModelState.AddModelError("DeleteConfirmPassword", "La contraseña no es correcta");
                    OnGet();
                    return Page();
                }

                // Eliminar el usuario y todas sus relaciones en una transacción
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Eliminar registros relacionados
                        var autos = await _context.Autos
                            .Where(a => a.EmailPropietario == currentUserEmail)
                            .ToListAsync();
                        _context.Autos.RemoveRange(autos);
                        
                        var favoritos = await _context.Favoritos
                            .Where(f => f.EmailUsuario == currentUserEmail)
                            .ToListAsync();
                        _context.Favoritos.RemoveRange(favoritos);
                        
                        // Para mensajes y reclamos, solo eliminar donde el usuario es cliente
                        // Si es admin, mejor poner null en vez de eliminar los registros
                        var mensajesCliente = await _context.Mensajes
                            .Where(m => m.EmailCliente == currentUserEmail)
                            .ToListAsync();
                        _context.Mensajes.RemoveRange(mensajesCliente);
                        
                        var mensajesAdmin = await _context.Mensajes
                            .Where(m => m.EmailAdminRespuesta == currentUserEmail)
                            .ToListAsync();
                        foreach (var mensaje in mensajesAdmin)
                        {
                            mensaje.EmailAdminRespuesta = null;
                        }
                        
                        var reclamosCliente = await _context.Reclamos
                            .Where(r => r.EmailCliente == currentUserEmail)
                            .ToListAsync();
                        _context.Reclamos.RemoveRange(reclamosCliente);
                        
                        var reclamosAdmin = await _context.Reclamos
                            .Where(r => r.EmailAdminRespuesta == currentUserEmail)
                            .ToListAsync();
                        foreach (var reclamo in reclamosAdmin)
                        {
                            reclamo.EmailAdminRespuesta = null;
                        }
                        
                        await _context.SaveChangesAsync();

                        // Eliminar el usuario
                        _context.Usuarios.Remove(usuario);
                        await _context.SaveChangesAsync();

                        // Commit de la transacción
                        await transaction.CommitAsync();

                        // Redireccionar a página de despedida o login
                        TempData["AccountDeleted"] = "Tu cuenta ha sido eliminada exitosamente";
                        return RedirectToPage("/Index");
                    }
                    catch (Exception ex)
                    {
                        // Rollback en caso de error
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", $"Error al eliminar la cuenta: {ex.Message}");
                        OnGet();
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ha ocurrido un error al eliminar la cuenta: {ex.Message}");
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
            catch (Exception)
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
            catch (Exception)
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

        private async Task UpdateSessionEmailAsync(string newEmail)
        {
            try
            {
                // Obtener el usuario actualizado
                var usuario = await _context.Usuarios.FindAsync(newEmail);
                if (usuario == null) return;

                // Crear los nuevos claims con el email actualizado
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Email),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, usuario.Nombre ?? usuario.Email)
                };

                if (usuario.EsAgencia)
                {
                    claims.Add(new Claim("IsAgency", "true"));
                    claims.Add(new Claim("AgencyName", usuario.NombreAgencia!));
                }

                if (usuario.EsAdministrador)
                {
                    claims.Add(new Claim("IsAdmin", "true"));
                    claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                }

                // Crear nueva identidad con los claims actualizados
                var claimsIdentity = new ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                };

                // Actualizar la sesión con los nuevos claims
                await HttpContext.SignInAsync(
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - la actualización de la base de datos ya se completó
                Console.WriteLine($"Error updating session: {ex.Message}");
            }
        }
    }
}
