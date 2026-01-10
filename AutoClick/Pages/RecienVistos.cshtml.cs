using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;

namespace AutoClick.Pages;

public class RecienVistosModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IBanderinesService _banderinesService;

    public RecienVistosModel(ApplicationDbContext context, IBanderinesService banderinesService)
    {
        _context = context;
        _banderinesService = banderinesService;
    }

    public IList<Auto> RecentlyViewedAutos { get; set; } = new List<Auto>();
    public Dictionary<int, List<string>> BanderinUrls { get; set; } = new(); // Dictionary for multiple banderines
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 11; // Desktop: 11 cards (3x4 grid - 1 for ad)
    public string SortBy { get; set; } = "recent";
    public Dictionary<int, int> AutoCountByOwner { get; set; } = new Dictionary<int, int>();
    public bool IsMobile { get; set; }
    public bool IsTablet { get; set; }

    public async Task OnGetAsync(int? page, string? sortBy)
    {
        CurrentPage = page ?? 1;
        SortBy = sortBy ?? "recent";

        // Detectar dispositivo móvil o tablet
        var userAgent = Request.Headers["User-Agent"].ToString().ToLower();
        IsMobile = userAgent.Contains("mobile") && !userAgent.Contains("tablet");
        IsTablet = userAgent.Contains("tablet") || (userAgent.Contains("android") && !userAgent.Contains("mobile"));

        // Ajustar PageSize según dispositivo
        if (IsMobile)
        {
            PageSize = 5; // Móvil: 5 cards
        }
        else if (IsTablet)
        {
            PageSize = 8; // Tablet: 8 cards
        }
        else
        {
            PageSize = 11; // Desktop: 11 cards
        }

        // Por ahora mostraremos todos los autos como "recién vistos"
        // En una implementación real, tendrías un sistema de tracking de vistas
        var query = _context.Autos
            .Include(a => a.Propietario)
            .Where(a => a.Activo);
        
        // Apply sorting
        query = SortBy switch
        {
            "price-asc" => query.OrderBy(a => a.Precio),
            "price-desc" => query.OrderByDescending(a => a.Precio),
            "year" => query.OrderByDescending(a => a.Ano),
            _ => query.OrderByDescending(a => a.FechaActualizacion) // "recent"
        };

        var totalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

        RecentlyViewedAutos = await query
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
        
        // Calcular cantidad de autos por propietario
        foreach (var auto in RecentlyViewedAutos)
        {
            if (!string.IsNullOrEmpty(auto.EmailPropietario))
            {
                var count = await _context.Autos.CountAsync(a => a.EmailPropietario == auto.EmailPropietario && a.Activo);
                AutoCountByOwner[auto.Id] = count;
            }
        }
        
        // Cargar URLs de banderines
        await LoadBanderinUrlsAsync(RecentlyViewedAutos.ToList());
        
        // Si no hay autos en la base de datos, crear algunos ejemplos
        if (!RecentlyViewedAutos.Any())
        {
            RecentlyViewedAutos = GetSampleRecentlyViewedAutos();
        }
    }
    
    private async Task LoadBanderinUrlsAsync(List<Auto> autos)
    {
        foreach (var auto in autos)
        {
            if (!BanderinUrls.ContainsKey(auto.Id))
            {
                var urls = new List<string>();
                var banderinFiles = auto.BanderinesVideoUrls;
                
                foreach (var fileName in banderinFiles)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var url = await _banderinesService.GetBanderinUrlAsync(fileName);
                        if (!string.IsNullOrEmpty(url))
                        {
                            urls.Add(url);
                        }
                    }
                }
                
                if (urls.Count > 0)
                {
                    BanderinUrls[auto.Id] = urls;
                }
            }
        }
    }

    private List<Auto> GetSampleRecentlyViewedAutos()
    {
        return new List<Auto>
        {
            new Auto { Id = 1, Marca = "Mercedes-Benz", Modelo = "E53 Coupé", Ano = 2022, Precio = 170000, ImagenPrincipal = "https://placehold.co/392x209", Carroceria = "Coupé", Combustible = "Gasolina", Transmision = "Automática", NumeroPuertas = 2, Provincia = "San José", Canton = "Escazú", PlacaVehiculo = "MER001", Condicion = "Excelente", EmailPropietario = "mercedes@dealer.com" },
            new Auto { Id = 2, Marca = "BMW", Modelo = "X5 M50i", Ano = 2023, Precio = 95000, ImagenPrincipal = "https://placehold.co/392x209", Carroceria = "SUV", Combustible = "Gasolina", Transmision = "Automática", NumeroPuertas = 4, Provincia = "San José", Canton = "Santa Ana", PlacaVehiculo = "BMW002", Condicion = "Excelente", EmailPropietario = "bmw@dealer.com" }
        };
    }
}
