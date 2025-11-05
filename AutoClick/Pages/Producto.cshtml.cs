using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using AutoClick.Services;

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
        
        // Financing Calculator Properties
        [BindProperty(SupportsGet = true)]
        public string Currency { get; set; } = "USD";
        
        [BindProperty(SupportsGet = true)]
        public decimal VehiclePrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int LoanTerm { get; set; } = 84;
        
        [BindProperty(SupportsGet = true)]
        public decimal DownPayment { get; set; } = 10000;
        
        [BindProperty(SupportsGet = true)]
        public decimal AnnualRate { get; set; } = 8.5m;
        
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

            // Verificar si el usuario es admin
            var isAdmin = User.IsInRole("Admin") || User.HasClaim("Role", "Admin");

            // Intentar cargar el auto real desde la base de datos
            // Si es admin, permitir ver anuncios pendientes de aprobación
            Vehicle = await _context.Autos
                .Include(a => a.Propietario)
                .FirstOrDefaultAsync(a => a.Id == Id && a.Activo && (isAdmin || a.PlanVisibilidad > 0));
            
            // Si no se encuentra, usar datos de muestra como fallback
            if (Vehicle == null)
            {
                var sampleAutos = GetSampleAutos();
                Vehicle = sampleAutos.FirstOrDefault(a => a.Id == Id);
                
                if (Vehicle == null)
                {
                    Vehicle = CreateDefaultAuto();
                }
            }

            if (VehiclePrice == 0)
                VehiclePrice = Vehicle.Precio;

            // Always keep AnnualRate fixed at 8.5%
            AnnualRate = 8.5m;

            CalculateMonthlyPayment();
            await LoadSimilarAutos();
            
            // Cargar análisis de valor de mercado
            if (Vehicle != null)
            {
                ValorMercado = await _ventasExternasService.AnalizarValorMercado(Vehicle);
            }

            return Page();
        }

        private Auto CreateDefaultAuto()
        {
            return new Auto
            {
                Id = Id,
                Marca = "Mercedes-Benz",
                Modelo = "E53 Coupe",
                Ano = 2022,
                Precio = 170000,
                Kilometraje = 15000,
                Carroceria = "Coupe",
                Combustible = "Gasolina",
                Transmision = "Automatica",
                NumeroPuertas = 2,
                Cilindrada = "3.0L V6 Turbo",
                ExtrasInterior = "[\"Sistema de navegacion\",\"Bluetooth\"]",
                ExtrasExterior = "[\"Camara de reversa\",\"Sensores\"]",
                Provincia = "San Jose",
                Canton = "San Jose",
                PlacaVehiculo = "BCR123",
                Condicion = "Excelente",
                EmailPropietario = "dealer@mercedes.com"
            };
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
            var similarFromDb = candidates
                .OrderBy(a => Math.Abs(a.Precio - Vehicle.Precio))
                .Take(3)
                .ToList();

            // Si no hay suficientes autos similares en la DB, completar con datos de muestra
            if (similarFromDb.Count < 3)
            {
                var sampleAutos = GetSampleAutos()
                    .Where(a => a.Id != Id)
                    .Take(3 - similarFromDb.Count)
                    .ToList();
                
                similarFromDb.AddRange(sampleAutos);
            }

            SimilarAutos = similarFromDb.Take(3).ToList();
        }

        private List<Auto> GetSampleAutos()
        {
            return new List<Auto>
            {
                new Auto
                {
                    Id = 1,
                    Marca = "Mercedes-Benz",
                    Modelo = "E53 AMG Coupe",
                    Ano = 2022,
                    Precio = 150000,
                    ImagenPrincipal = "https://placehold.co/803x462",
                    Kilometraje = 22000,
                    Carroceria = "Coupe",
                    Combustible = "Hibrida",
                    Transmision = "Automatica",
                    NumeroPuertas = 2,
                    Cilindrada = "3.000 cc",
                    Provincia = "San Jose",
                    Canton = "Escazu",
                    PlacaVehiculo = "MER001",
                    Condicion = "Excelente",
                    EmailPropietario = "mercedes@dealer.com"
                },
                new Auto
                {
                    Id = 2,
                    Marca = "BMW",
                    Modelo = "X5M",
                    Ano = 2023,
                    Precio = 170000,
                    ImagenPrincipal = "https://placehold.co/392x209",
                    Kilometraje = 15000,
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automatica",
                    NumeroPuertas = 4,
                    Provincia = "San Jose",
                    Canton = "Santa Ana",
                    PlacaVehiculo = "BMW002",
                    Condicion = "Excelente",
                    EmailPropietario = "bmw@dealer.com"
                }
            };
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
                Console.WriteLine($"Error al crear solicitud de pre-aprobación: {ex.Message}");
                return new JsonResult(new { success = false, message = "Ocurrió un error al procesar su solicitud. Por favor intente nuevamente." });
            }
        }
    }
}
