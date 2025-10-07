using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoClick.Pages
{
    public class RegistroAgenciaUbicacionModel : PageModel
    {
        public void OnGet()
        {
            // Verificar si hay información de registro exitoso en sesión
            var nombreAgencia = HttpContext.Session.GetString("NombreAgencia");
            if (!string.IsNullOrEmpty(nombreAgencia))
            {
                ViewData["NombreAgencia"] = nombreAgencia;
            }
        }
    }
}