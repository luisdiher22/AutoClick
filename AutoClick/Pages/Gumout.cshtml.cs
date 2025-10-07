using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages
{
    public class GumoutModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "La moneda es requerida")]
        public string? Moneda { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El precio del vehículo es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal? PrecioVehiculo { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El plazo del préstamo es requerido")]
        public int? Plazo { get; set; }

        [BindProperty]
        public decimal? Prima { get; set; }

        [BindProperty]
        public decimal? TasaAnual { get; set; }

        [BindProperty]
        public decimal? CuotaMensual { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Por favor complete todos los campos requeridos correctamente.";
                return Page();
            }

            try
            {
                // Calculate loan details if all required fields are provided
                if (PrecioVehiculo.HasValue && Plazo.HasValue)
                {
                    var tasa = TasaAnual ?? 8.95m; // Default interest rate
                    var prima = Prima ?? 0;
                    var montoPrestamo = PrecioVehiculo.Value - prima;

                    if (montoPrestamo > 0 && Plazo.Value > 0)
                    {
                        // Calculate monthly payment using loan formula
                        var tasaMensual = (double)(tasa / 100 / 12);
                        var numeroPagos = Plazo.Value;
                        
                        if (tasaMensual > 0)
                        {
                            var cuota = (double)montoPrestamo * 
                                       (tasaMensual * Math.Pow(1 + tasaMensual, numeroPagos)) /
                                       (Math.Pow(1 + tasaMensual, numeroPagos) - 1);
                            
                            CuotaMensual = Math.Round((decimal)cuota, 2);
                        }
                        else
                        {
                            CuotaMensual = montoPrestamo / Plazo.Value;
                        }
                    }
                }

                // Simulate pre-approval request processing
                await Task.Delay(100);

                SuccessMessage = $"¡Solicitud de pre-aprobación enviada exitosamente! " +
                               $"Nuestro equipo de Gumout se comunicará contigo en las próximas 24 horas para continuar con el proceso. " +
                               $"Cuota mensual estimada: ${CuotaMensual:N2} {Moneda}.";
                
                // Clear form after successful submission
                ModelState.Clear();
                Moneda = null;
                PrecioVehiculo = null;
                Plazo = null;
                Prima = null;
                TasaAnual = null;

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ocurrió un error al procesar tu solicitud: {ex.Message}. Por favor intenta nuevamente.";
                return Page();
            }
        }
    }
}