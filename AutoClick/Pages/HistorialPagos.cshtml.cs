using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    [Authorize]
    public class HistorialPagosModel : PageModel
    {
        public List<PaymentRecord> PaymentHistory { get; set; } = new();

        [BindProperty]
        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido")]
        public string NewsletterEmail { get; set; } = string.Empty;

        public void OnGet()
        {
            // Simular datos del historial de pagos
            PaymentHistory = new List<PaymentRecord>
            {
                new PaymentRecord
                {
                    Date = DateTime.Parse("2025-06-03"),
                    Detail = "Posteo Instantáneo",
                    PaymentMethod = "Tarjeta débito/crédito",
                    Amount = 2500,
                    Status = "Aprobado"
                },
                new PaymentRecord
                {
                    Date = DateTime.Parse("2025-06-03"),
                    Detail = "Posteo Instantáneo",
                    PaymentMethod = "Tarjeta débito/crédito",
                    Amount = 2500,
                    Status = "Aprobado"
                },
                new PaymentRecord
                {
                    Date = DateTime.Parse("2025-06-03"),
                    Detail = "Posteo Instantáneo",
                    PaymentMethod = "Tarjeta débito/crédito",
                    Amount = 2500,
                    Status = "Aprobado"
                },
                new PaymentRecord
                {
                    Date = DateTime.Parse("2025-06-03"),
                    Detail = "Posteo Instantáneo",
                    PaymentMethod = "Tarjeta débito/crédito",
                    Amount = 2500,
                    Status = "Aprobado"
                },
                new PaymentRecord
                {
                    Date = DateTime.Parse("2025-06-03"),
                    Detail = "Posteo Instantáneo",
                    PaymentMethod = "Tarjeta débito/crédito",
                    Amount = 2500,
                    Status = "Aprobado"
                }
            };
        }

        public async Task<IActionResult> OnPostNewsletterAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet(); // Reload payment history
                return Page();
            }

            try
            {
                // Aquí se implementaría la lógica de suscripción al newsletter
                // Por ejemplo, guardar el email en la base de datos

                // Simular delay de procesamiento
                await Task.Delay(500);

                TempData["NewsletterSuccess"] = "¡Te has suscrito exitosamente al newsletter!";
                return RedirectToPage();
            }
            catch (Exception)
            {
                // Log del error
                ModelState.AddModelError("NewsletterEmail", "Ha ocurrido un error al procesar tu suscripción. Intenta de nuevo.");
                OnGet(); // Reload payment history
                return Page();
            }
        }
    }

    public class PaymentRecord
    {
        public DateTime Date { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;

        public string FormattedAmount => $"₡{Amount:N0}";

        public string StatusClass => Status.ToLower() switch
        {
            "aprobado" => "approved",
            "pendiente" => "pending",
            "rechazado" => "rejected",
            _ => "unknown"
        };
    }
}