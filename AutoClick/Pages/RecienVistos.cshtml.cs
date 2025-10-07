using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.Pages;

public class RecienVistosModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public RecienVistosModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Auto> RecentlyViewedAutos { get; set; } = new List<Auto>();

    public async Task OnGetAsync()
    {
        // Por ahora mostraremos todos los autos como "recién vistos"
        // En una implementación real, tendrías un sistema de tracking de vistas
        RecentlyViewedAutos = await _context.Autos
            .OrderByDescending(a => a.FechaActualizacion)
            .Take(12) // Mostrar 12 autos en grid de 3x4
            .ToListAsync();
        
        // Si no hay autos en la base de datos, crear algunos ejemplos
        if (!RecentlyViewedAutos.Any())
        {
            RecentlyViewedAutos = GetSampleRecentlyViewedAutos();
        }
    }

    private List<Auto> GetSampleRecentlyViewedAutos()
    {
        return new List<Auto>
        {
            new Auto { Id = 1, Marca = "Mercedes-Benz", Modelo = "E53 Coupé", Ano = 2022, ValorFiscal = 170000, ImagenPrincipal = "https://placehold.co/392x209", Carroceria = "Coupé", Combustible = "Gasolina", Transmision = "Automática", NumeroPuertas = 2, Provincia = "San José", Canton = "Escazú", PlacaVehiculo = "MER001", Condicion = "Excelente", EmailPropietario = "mercedes@dealer.com" },
            new Auto { Id = 2, Marca = "BMW", Modelo = "X5 M50i", Ano = 2023, ValorFiscal = 95000, ImagenPrincipal = "https://placehold.co/392x209", Carroceria = "SUV", Combustible = "Gasolina", Transmision = "Automática", NumeroPuertas = 4, Provincia = "San José", Canton = "Santa Ana", PlacaVehiculo = "BMW002", Condicion = "Excelente", EmailPropietario = "bmw@dealer.com" }
        };
    }
}