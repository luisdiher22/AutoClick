using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Services;

namespace AutoClick.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAutoService _autoService;

    public IndexModel(ILogger<IndexModel> logger, IAutoService autoService)
    {
        _logger = logger;
        _autoService = autoService;
    }

    public List<Auto> AutosDestacados { get; set; } = new();
    public List<Auto> AutosRecientes { get; set; } = new();
    public List<Auto> AutosGuardados { get; set; } = new();
    public List<Auto> AutosExploracion { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            // Obtener autos destacados usando el servicio
            AutosDestacados = await _autoService.GetAutosDestacadosAsync(3);
            
            // Si no hay autos destacados, usar los más recientes como destacados
            if (!AutosDestacados.Any())
            {
                AutosDestacados = await _autoService.GetAutosRecientesAsync(3);
            }

            // Obtener autos más recientes
            AutosRecientes = await _autoService.GetAutosRecientesAsync(3);

            // Para autos guardados, usar todos los autos disponibles
            // (en el futuro se implementará un sistema de favoritos por usuario)
            AutosGuardados = await _autoService.GetAutosRecientesAsync(3);

            // Para exploración general
            AutosExploracion = await _autoService.GetAutosAleatoriosAsync(3);

            // Si no hay datos, usar valores por defecto
            if (!AutosDestacados.Any()) AutosDestacados = GetSampleAutos().Take(3).ToList();
            if (!AutosRecientes.Any()) AutosRecientes = GetSampleAutos().Take(3).ToList();
            if (!AutosGuardados.Any()) AutosGuardados = GetSampleAutos().Take(3).ToList();
            if (!AutosExploracion.Any()) AutosExploracion = GetSampleAutos().Take(3).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar autos en la página principal");
            
            // Fallback a datos de muestra
            var sampleAutos = GetSampleAutos();
            AutosDestacados = sampleAutos.Take(3).ToList();
            AutosRecientes = sampleAutos.Skip(3).Take(3).ToList();
            AutosGuardados = sampleAutos.Take(3).ToList();
            AutosExploracion = sampleAutos.Skip(6).Take(3).ToList();
        }
    }

    private List<Auto> GetSampleAutos()
    {
        return new List<Auto>
        {
            new Auto
            {
                Id = 1,
                Marca = "Toyota",
                Modelo = "Corolla",
                Ano = 2023,
                ValorFiscal = 25000,
                ImagenPrincipal = "/uploads/autos-images/sample-corolla.jpg",
                Carroceria = "Sedan",
                Combustible = "Gasolina",
                Transmision = "Automática",
                NumeroPuertas = 4,
                Provincia = "San José",
                Canton = "Escazú",
                PlacaVehiculo = "TOY001",
                Condicion = "Nuevo",
                EmailPropietario = "admin@gmail.com",
                FechaCreacion = DateTime.Now.AddDays(-1),
                PlanVisibilidad = 2
            },
            new Auto
            {
                Id = 2,
                Marca = "Mercedes-Benz",
                Modelo = "E53 AMG",
                Ano = 2022,
                ValorFiscal = 170000,
                ImagenPrincipal = "/uploads/autos-images/sample-mercedes.jpg",
                Carroceria = "Coupé",
                Combustible = "Gasolina",
                Transmision = "Automática",
                NumeroPuertas = 2,
                Provincia = "San José",
                Canton = "Santa Ana",
                PlacaVehiculo = "MER002",
                Condicion = "Excelente",
                EmailPropietario = "admin@gmail.com",
                FechaCreacion = DateTime.Now.AddDays(-2),
                PlanVisibilidad = 3
            },
            new Auto
            {
                Id = 3,
                Marca = "Audi",
                Modelo = "Q5 Sportback",
                Ano = 2022,
                ValorFiscal = 75000,
                ImagenPrincipal = "/uploads/autos-images/sample-audi.jpg",
                Carroceria = "SUV",
                Combustible = "Gasolina",
                Transmision = "Automática",
                NumeroPuertas = 4,
                Provincia = "Alajuela",
                Canton = "Alajuela",
                PlacaVehiculo = null,
                Condicion = "Nuevo",
                EmailPropietario = "admin@gmail.com",
                FechaCreacion = DateTime.Now.AddDays(-3),
                PlanVisibilidad = 1
            }
        };
    }
}
