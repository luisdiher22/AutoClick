using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using AutoClick.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace AutoClick.Pages
{
    public class RegistroAgenciaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAuthService _authService;

        public RegistroAgenciaModel(ApplicationDbContext context, IFileUploadService fileUploadService, IAuthService authService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _authService = authService;
        }

        [BindProperty]
        [Required(ErrorMessage = "El nombre de la agencia es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre de la agencia no puede exceder 100 caracteres")]
        public string NombreAgencia { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Nombre { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [MaxLength(100, ErrorMessage = "Los apellidos no pueden exceder 100 caracteres")]
        public string Apellidos { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [MaxLength(150, ErrorMessage = "El correo no puede exceder 150 caracteres")]
        public string Email { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar el correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [Compare("Email", ErrorMessage = "Los correos electrónicos no coinciden")]
        public string ConfirmarEmail { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Telefono1 { get; set; } = "";

        [BindProperty]
        [Phone(ErrorMessage = "Ingrese un número de WhatsApp válido")]
        [MaxLength(20, ErrorMessage = "El WhatsApp no puede exceder 20 caracteres")]
        public string? WhatsApp { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string Contrasena { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; } = "";

        [BindProperty]
        [MaxLength(50, ErrorMessage = "La cédula no puede exceder 50 caracteres")]
        public string? CedulaJuridica { get; set; }

        [BindProperty]
        [MaxLength(50, ErrorMessage = "La provincia no puede exceder 50 caracteres")]
        public string? Provincia { get; set; }

        [BindProperty]
        [MaxLength(50, ErrorMessage = "El cantón no puede exceder 50 caracteres")]
        public string? Canton { get; set; }

        [BindProperty]
        public IFormFile? ImagenPerfil { get; set; }

        [BindProperty]
        public IFormFile? ImagenBanner { get; set; }

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public void OnGet()
        {
            // Verificar si hay un tipo de usuario seleccionado en la sesión
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");
            if (string.IsNullOrEmpty(tipoUsuario) || tipoUsuario != "Agencia")
            {
                // Si no hay sesión válida, redirigir a Auth
                Response.Redirect("/Auth");
                return;
            }

            // Limpiar mensajes
            ErrorMessage = "";
            SuccessMessage = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Limpiar mensajes previos
                ErrorMessage = "";
                SuccessMessage = "";

                // Validar que el modelo es válido
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    ErrorMessage = string.Join(". ", errors);
                    return Page();
                }

                // Validaciones adicionales
                if (Email.ToLower() != ConfirmarEmail.ToLower())
                {
                    ErrorMessage = "Los correos electrónicos no coinciden";
                    return Page();
                }

                if (Contrasena != ConfirmarContrasena)
                {
                    ErrorMessage = "Las contraseñas no coinciden";
                    return Page();
                }

                // Verificar si el usuario ya existe
                var existingUser = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == Email.ToLower());

                if (existingUser != null)
                {
                    ErrorMessage = "Ya existe una cuenta con este correo electrónico";
                    return Page();
                }

                // Subir imágenes si se proporcionaron
                string? imagenPerfilUrl = null;
                string? imagenBannerUrl = null;

                if (ImagenPerfil != null && ImagenPerfil.Length > 0)
                {
                    try
                    {
                        imagenPerfilUrl = await _fileUploadService.UploadFileAsync(ImagenPerfil, "agencias");
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = "Error al subir la imagen de perfil. Intente nuevamente.";
                        return Page();
                    }
                }

                if (ImagenBanner != null && ImagenBanner.Length > 0)
                {
                    try
                    {
                        imagenBannerUrl = await _fileUploadService.UploadFileAsync(ImagenBanner, "agencias");
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = "Error al subir la imagen del banner. Intente nuevamente.";
                        return Page();
                    }
                }

                // Crear el nuevo usuario (agencia)
                var nuevoUsuario = new Usuario
                {
                    Email = Email.ToLower().Trim(),
                    Nombre = Nombre.Trim(),
                    Apellidos = Apellidos.Trim(),
                    NumeroTelefono = Telefono1.Trim(),
                    Contrasena = HashPassword(Contrasena),
                    NombreAgencia = NombreAgencia.Trim(), // Esto hace que EsAgencia sea true
                    CedulaJuridica = CedulaJuridica?.Trim(),
                    Provincia = Provincia?.Trim(),
                    Canton = Canton?.Trim(),
                    ImagenPerfilUrl = imagenPerfilUrl,
                    ImagenBannerUrl = imagenBannerUrl
                };

                // Guardar en la base de datos
                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                // Iniciar sesión automáticamente usando AuthService
                var loginResult = await _authService.LoginAsync(nuevoUsuario.Email, Contrasena);
                
                if (!loginResult.Success)
                {
                    ErrorMessage = "Cuenta creada pero hubo un error al iniciar sesión. Por favor, inicie sesión manualmente.";
                    return RedirectToPage("/Auth");
                }

                // Limpiar la sesión de tipo de usuario
                HttpContext.Session.Remove("TipoUsuario");

                // Verificar si hay una URL de retorno
                var returnUrl = HttpContext.Session.GetString("ReturnUrl");
                HttpContext.Session.Remove("ReturnUrl"); // Limpiar después de usar
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirigir a la página principal
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                // Log del error en una aplicación real

                ErrorMessage = "Ocurrió un error al crear la cuenta. Intente nuevamente.";
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            // En una aplicación real, usar un sistema más robusto como BCrypt o Identity
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "SALT_KEY_AUTOCLICK"));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
