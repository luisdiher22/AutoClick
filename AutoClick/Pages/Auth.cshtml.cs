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

        public AuthModel(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public string TipoUsuario { get; set; } = "";

        [BindProperty]
        public string LoginEmail { get; set; } = "";

        [BindProperty]
        public string LoginPassword { get; set; } = "";

        [BindProperty]
        public bool RememberMe { get; set; } = false;

        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

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
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex.Message}");
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
                    // Redirect to home page or return URL
                    string? returnUrl = Request.Query["returnUrl"];
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
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
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
    }
}