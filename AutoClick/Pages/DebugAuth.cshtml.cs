using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class DebugAuthModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public DebugAuthModel(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public string Message { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Delete existing admin user
                var existingAdmin = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
                if (existingAdmin != null)
                {
                    _context.Usuarios.Remove(existingAdmin);
                    await _context.SaveChangesAsync();
                    Message += "Usuario admin eliminado. ";
                }

                // Create new admin user with correct password
                var adminUser = new Usuario
                {
                    Email = "admin@gmail.com",
                    Nombre = "Admin",
                    Apellidos = "User",
                    NumeroTelefono = "12345678",
                    NombreAgencia = null
                };

                var result = await _authService.RegisterAsync(adminUser, "prueba123");
                if (result.Success)
                {
                    Message += "Usuario admin creado correctamente con contraseña 'prueba123'";
                }
                else
                {
                    Message += $"Error creando usuario admin: {result.Message}";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
            }

            return Page();
        }
    }
}