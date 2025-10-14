using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoClick.Pages.Admin
{
    public class VisitasMensualesModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Verificar si el usuario es administrador
            var isAdminClaim = User.FindFirst("IsAdmin");
            if (isAdminClaim?.Value != "true")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}