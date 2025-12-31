using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoClick.Pages.Pagos
{
    public class ProcessPaymentModel : PageModel
    {
        public void OnGet(int? autoId, int? anuncioId, int? amount, string? currency, string? description)
        {
            ViewData["AutoId"] = autoId;
            ViewData["AnuncioId"] = anuncioId;
            ViewData["Amount"] = amount ?? 0;
            ViewData["Currency"] = currency ?? "CRC";
            ViewData["Description"] = description ?? "Pago AutoClick";
        }
    }
}
