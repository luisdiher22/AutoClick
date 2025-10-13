using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages.Admin
{
    public class SetupAdminModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SetupAdminModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Message { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Buscar el usuario admin@gmail.com
                var adminUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
                
                if (adminUser != null)
                {
                    // Actualizar para que sea administrador
                    adminUser.EsAdministrador = true;
                    await _context.SaveChangesAsync();
                    Message = "Usuario admin@gmail.com actualizado correctamente como administrador.";
                }
                else
                {
                    Message = "Usuario admin@gmail.com no encontrado. Debe registrarse primero.";
                }
            }
            catch (Exception ex)
            {
                Message = $"Error al actualizar usuario: {ex.Message}";
            }

            return Page();
        }
    }
}