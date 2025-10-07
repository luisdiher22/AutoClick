using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class RegistroAgenciaModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El nombre comercial es requerido")]
        [Display(Name = "Nombre Comercial")]
        public string NombreComercial { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La cédula jurídica es requerida")]
        [Display(Name = "Cédula Jurídica/Física")]
        public string CedulaJuridica { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El nombre del representante legal es requerido")]
        [Display(Name = "Representante Legal")]
        public string RepresentanteLegal { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar el correo electrónico")]
        [Compare("Email", ErrorMessage = "Los correos electrónicos no coinciden")]
        [Display(Name = "Confirmar Email")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "El teléfono principal es requerido")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [Display(Name = "Teléfono 1")]
        public string Telefono1 { get; set; } = string.Empty;

        [BindProperty]
        [Phone(ErrorMessage = "El formato del WhatsApp no es válido")]
        [Display(Name = "WhatsApp")]
        public string? WhatsApp { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public void OnGet()
        {
            // Inicialización de la página
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Validar que la cédula jurídica no esté ya registrada
                if (await CedulaYaRegistrada(CedulaJuridica))
                {
                    ModelState.AddModelError("CedulaJuridica", "Esta cédula jurídica ya está registrada");
                    return Page();
                }

                // Validar que el email no esté ya registrado
                if (await EmailYaRegistrado(Email))
                {
                    ModelState.AddModelError("Email", "Este correo electrónico ya está registrado");
                    return Page();
                }

                // Aquí iría la lógica para guardar los datos de la agencia
                // Por ahora simularemos el proceso
                await GuardarDatosAgencia();

                // Establecer sesión para indicar que el registro fue exitoso
                HttpContext.Session.SetString("RegistroAgenciaExitoso", "true");
                HttpContext.Session.SetString("NombreAgencia", NombreComercial);

                // Redirigir a la siguiente página del proceso (paso 2)
                return RedirectToPage("/RegistroAgenciaUbicacion");
            }
            catch (Exception ex)
            {
                // Log del error (aquí usaríamos un logger real)
                Console.WriteLine($"Error durante el registro de agencia: {ex.Message}");
                
                ModelState.AddModelError("", "Ocurrió un error durante el registro. Por favor, intente nuevamente.");
                return Page();
            }
        }

        private async Task<bool> CedulaYaRegistrada(string cedula)
        {
            // Aquí iría la lógica para verificar en la base de datos
            // si la cédula jurídica ya está registrada
            await Task.Delay(100); // Simular consulta a BD
            
            // Por ahora retornamos false (cédula disponible)
            // En implementación real: return await _context.Agencias.AnyAsync(a => a.CedulaJuridica == cedula);
            return false;
        }

        private async Task<bool> EmailYaRegistrado(string email)
        {
            // Aquí iría la lógica para verificar en la base de datos
            // si el email ya está registrado
            await Task.Delay(100); // Simular consulta a BD
            
            // Por ahora retornamos false (email disponible)
            // En implementación real: return await _context.Agencias.AnyAsync(a => a.Email == email);
            return false;
        }

        private async Task GuardarDatosAgencia()
        {
            // Aquí iría la lógica para guardar los datos en la base de datos
            await Task.Delay(200); // Simular proceso de guardado
            
            // En implementación real:
            /*
            var agencia = new Agencia
            {
                NombreComercial = NombreComercial,
                CedulaJuridica = CedulaJuridica,
                RepresentanteLegal = RepresentanteLegal,
                Email = Email,
                Telefono1 = Telefono1,
                WhatsApp = WhatsApp,
                PasswordHash = HashPassword(Password), // Usar hashing seguro
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };

            _context.Agencias.Add(agencia);
            await _context.SaveChangesAsync();
            */
        }

        private string HashPassword(string password)
        {
            // Aquí iría la implementación real de hashing
            // Usar BCrypt, Argon2, o similar
            return password; // Placeholder - NO usar en producción
        }

        public bool HasValidationErrors()
        {
            return !ModelState.IsValid;
        }

        public string GetValidationErrorsJson()
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            return System.Text.Json.JsonSerializer.Serialize(errors);
        }
    }
}