using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using AutoClick.Services;
using AutoClick.Helpers;

namespace AutoClick.Pages
{
    public class ProductoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IVentasExternasService _ventasExternasService;

        public ProductoModel(ApplicationDbContext context, IVentasExternasService ventasExternasService)
        {
            _context = context;
            _ventasExternasService = ventasExternasService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Auto? Vehicle { get; set; }
        public List<Auto> SimilarAutos { get; set; } = new List<Auto>();
        
        // Market Value Analysis
        public ValorMercadoAnalisis? ValorMercado { get; set; }
        
        // Marchamo and Transfer Costs
        public MarchamoDesglose? MarchamoInfo { get; set; }
        public TraspasoCostos? TraspasoInfo { get; set; }
        
        // Financing Calculator Properties
        [BindProperty(SupportsGet = true)]
        public string Currency { get; set; } = "USD";
        
        [BindProperty(SupportsGet = true)]
        public decimal VehiclePrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int LoanTerm { get; set; } = 84;
        
        [BindProperty(SupportsGet = true)]
        public decimal DownPayment { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public decimal AnnualRate { get; set; } = 8.0m;
        
        public decimal MonthlyPayment { get; set; }
        
        // Seller Information
        public string SellerName => Vehicle?.Propietario?.NombreCompleto ?? (Vehicle?.Marca + " Dealer");
        public string SellerPhone => Vehicle?.Propietario?.NumeroTelefono?.ToString() ?? "+506 8888-8888";
        public string SellerLocation => Vehicle?.UbicacionCompleta ?? "Canton, Provincia";
        public int SellerCarCount => GetSellerCarCount();

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id <= 0)
            {
                return NotFound();
            }

            // Verificar si el usuario es admin (chequear múltiples formas de identificar admin)
            var isAdmin = User.IsInRole("Admin") || 
                          User.IsInRole("Administrator") || 
                          User.HasClaim("Role", "Admin") ||
                          User.HasClaim("Role", "Administrator") ||
                          User.HasClaim("IsAdmin", "true");

            // Intentar cargar el auto real desde la base de datos
            // Si es admin, permitir ver CUALQUIER auto (incluso inactivos/pendientes)
            if (isAdmin)
            {
                Vehicle = await _context.Autos
                    .Include(a => a.Propietario)
                    .FirstOrDefaultAsync(a => a.Id == Id);
            }
            else
            {
                // Para usuarios normales, solo mostrar autos activos con plan visible
                Vehicle = await _context.Autos
                    .Include(a => a.Propietario)
                    .FirstOrDefaultAsync(a => a.Id == Id && a.Activo && a.PlanVisibilidad > 0);
            }
            
            // Si no se encuentra el vehículo, retornar 404
            if (Vehicle == null)
            {
                return NotFound();
            }

            if (VehiclePrice == 0)
                VehiclePrice = Vehicle.Precio;

            // Set down payment to 20% of vehicle price (in CRC)
            var precioEnCRC = AutoClick.Helpers.PrecioHelper.ConvertirACRC(Vehicle.Precio, Vehicle.Divisa);
            if (DownPayment == 0)
                DownPayment = precioEnCRC * 0.20m;

            // Always keep AnnualRate fixed at 8%
            AnnualRate = 8.0m;

            CalculateMonthlyPayment();
            await LoadSimilarAutos();
            
            // Cargar análisis de valor de mercado
            if (Vehicle != null)
            {
                ValorMercado = await _ventasExternasService.AnalizarValorMercado(Vehicle);
                
                // Calcular marchamo y costos de traspaso
                CalcularMarchamoYTraspaso();
            }

            return Page();
        }

        private void CalculateMonthlyPayment()
        {
            if (VehiclePrice > 0 && LoanTerm > 0 && AnnualRate > 0)
            {
                var loanAmount = VehiclePrice - DownPayment;
                var monthlyRate = (double)(AnnualRate / 100) / 12;
                var numPayments = LoanTerm;

                if (loanAmount > 0)
                {
                    var monthlyPaymentCalc = loanAmount * 
                        (decimal)(monthlyRate * Math.Pow(1 + monthlyRate, numPayments)) /
                        (decimal)(Math.Pow(1 + monthlyRate, numPayments) - 1);
                    
                    MonthlyPayment = Math.Round(monthlyPaymentCalc, 2);
                }
            }
        }

        private void CalcularMarchamoYTraspaso()
        {
            if (Vehicle == null) return;

            // Solo calcular si hay valor fiscal disponible
            if (Vehicle.ValorFiscal.HasValue && Vehicle.ValorFiscal.Value > 0)
            {
                // Calcular marchamo
                MarchamoInfo = MarchamoHelper.CalcularMarchamoDesglose(Vehicle.ValorFiscal.Value, Vehicle.Ano);
            }

            // Calcular costos de traspaso basado en el precio de venta
            if (Vehicle.Precio > 0)
            {
                // Convertir precio a CRC si está en USD
                decimal precioEnCRC = PrecioHelper.ConvertirACRC(Vehicle.Precio, Vehicle.Divisa);
                TraspasoInfo = MarchamoHelper.CalcularTraspasoDesglose(precioEnCRC);
            }
        }

        private async Task LoadSimilarAutos()
        {
            if (Vehicle == null)
            {
                SimilarAutos = new List<Auto>();
                return;
            }

            // Buscar autos similares en la base de datos (solo aprobados)
            // Primero obtenemos los candidatos sin ordenamiento complejo
            var candidates = await _context.Autos
                .Where(a => a.Id != Id && a.Activo && a.PlanVisibilidad > 0) // Excluir pendientes
                .Where(a => a.Marca == Vehicle.Marca || a.Carroceria == Vehicle.Carroceria)
                .ToListAsync();

            // Ordenar por diferencia de precio usando evaluación del cliente
            SimilarAutos = candidates
                .OrderBy(a => Math.Abs(a.Precio - Vehicle.Precio))
                .Take(3)
                .ToList();
        }

        public string FormatPrice(decimal price)
        {
            return price.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        public string FormatNumber(decimal number)
        {
            return number.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-CR"));
        }

        public string FormatKilometrage(int km)
        {
            return km.ToString("N0") + " km";
        }

        public string ToTitleCase(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
                
            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        public decimal GetMarketValue()
        {
            if (Vehicle != null)
            {
                return Vehicle.Precio + 4875;
            }
            return 0;
        }

        public string GetMarketStatus()
        {
            return "Trato justo";
        }

        private int GetSellerCarCount()
        {
            if (Vehicle?.EmailPropietario == null) return 15; // Default fallback

            try
            {
                return _context.Autos
                    .Where(a => a.EmailPropietario == Vehicle.EmailPropietario && a.Activo)
                    .Count();
            }
            catch
            {
                return 15; // Fallback en caso de error
            }
        }

        /// <summary>
        /// Handler para crear solicitud de pre-aprobación
        /// </summary>
        public async Task<IActionResult> OnPostSolicitarPreAprobacionAsync(
            [FromForm] string nombre,
            [FromForm] string apellidos,
            [FromForm] string telefono,
            [FromForm] string email,
            [FromForm] int autoId)
        {
            try
            {
                // Validar datos
                if (string.IsNullOrWhiteSpace(nombre) || 
                    string.IsNullOrWhiteSpace(apellidos) || 
                    string.IsNullOrWhiteSpace(telefono) || 
                    string.IsNullOrWhiteSpace(email))
                {
                    return new JsonResult(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que el auto existe
                var autoExists = await _context.Autos.AnyAsync(a => a.Id == autoId);
                if (!autoExists)
                {
                    return new JsonResult(new { success = false, message = "El vehículo no existe" });
                }

                // Crear la solicitud
                var solicitud = new SolicitudPreAprobacion
                {
                    Nombre = nombre.Trim(),
                    Apellidos = apellidos.Trim(),
                    Telefono = telefono.Trim(),
                    Email = email.Trim(),
                    AutoId = autoId,
                    FechaSolicitud = DateTime.Now,
                    Procesada = false
                };

                _context.SolicitudesPreAprobacion.Add(solicitud);
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true, message = "Su solicitud ha sido registrada exitosamente. Le contactaremos cuando tengamos una alianza bancaria disponible." });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { success = false, message = "Ocurrió un error al procesar su solicitud. Por favor intente nuevamente." });
            }
        }
    }
}
