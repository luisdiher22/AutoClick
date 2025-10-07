using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages;

public class TestLoginModel : PageModel
{
    private readonly IAuthService _authService;

    public TestLoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    public string Message { get; set; } = "";

    public void OnGet()
    {
        Message = "Click the button to test login with admin@gmail.com / prueba123";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            Message = "🔧 PASO 1: Eliminando usuario admin existente...\n";
            
            // Eliminar admin existente si existe
            var context = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var existingAdmin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
            if (existingAdmin != null)
            {
                context.Usuarios.Remove(existingAdmin);
                await context.SaveChangesAsync();
                Message += "✅ Usuario admin eliminado\n\n";
            }
            else
            {
                Message += "ℹ️ No había usuario admin existente\n\n";
            }
            
            Message += "🔧 PASO 2: Creando nuevo usuario admin...\n";
            
            // Crear nuevo usuario admin
            var adminUser = new Usuario
            {
                Email = "admin@gmail.com",
                Nombre = "Admin",
                Apellidos = "User", 
                NumeroTelefono = "12345678",
                NombreAgencia = null
            };
            
            var registerResult = await _authService.RegisterAsync(adminUser, "prueba123");
            if (registerResult.Success)
            {
                Message += "✅ Usuario admin creado exitosamente\n\n";
            }
            else
            {
                Message += $"❌ Error creando usuario admin: {registerResult.Message}\n\n";
                return Page();
            }
            
            Message += "🔧 PASO 3: Intentando login...\n";
            
            var result = await _authService.LoginAsync("admin@gmail.com", "prueba123");
            
            if (result.Success)
            {
                Message += "✅ LOGIN EXITOSO!\n\n";
                Message += "Usuario autenticado correctamente.\n";
                Message += $"Claims creados para: {User.Identity?.Name ?? "No identity"}";
                
                return RedirectToPage("/Index");
            }
            else
            {
                Message += "❌ LOGIN FALLIDO\n\n";
                Message += $"Mensaje: {result.Message}\n";
                Message += $"Errores: {string.Join(", ", result.Errors)}";
            }
        }
        catch (Exception ex)
        {
            Message += $"\n❌ ERROR INESPERADO\n\n{ex.Message}\n\n{ex.StackTrace}";
        }

        return Page();
    }
}