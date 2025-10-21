using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class ExplorarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExplorarModel(ApplicationDbContext context)
        {
            _context = context;
        }

            public List<Auto> Autos { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10; // 2 columns x 5 rows
        public string SortBy { get; set; } = "recent";
        
        // Filter properties
        public string? Province { get; set; }
        public string? Canton { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinKm { get; set; }
        public int? MaxKm { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }

        public async Task OnGetAsync(int? page, string? sortBy, string? province, string? canton, 
            string? brand, string? model, decimal? minPrice, decimal? maxPrice,
            int? minKm, int? maxKm, int? minYear, int? maxYear)
        {
            CurrentPage = page ?? 1;
            SortBy = sortBy ?? "recent";
            Province = province;
            Canton = canton;
            Brand = brand;
            Model = model;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            MinKm = minKm;
            MaxKm = maxKm;
            MinYear = minYear;
            MaxYear = maxYear;

            // Try to get cars from database
            try
            {
                var query = _context.Autos.AsQueryable();
                
                // Apply filters
                if (!string.IsNullOrEmpty(Brand))
                    query = query.Where(a => a.Marca.ToLower().Contains(Brand.ToLower()));
                if (!string.IsNullOrEmpty(Model))
                    query = query.Where(a => a.Modelo.ToLower().Contains(Model.ToLower()));
                if (MinPrice.HasValue)
                    query = query.Where(a => a.Precio >= MinPrice.Value);
                if (MaxPrice.HasValue)
                    query = query.Where(a => a.Precio <= MaxPrice.Value);
                if (MinYear.HasValue)
                    query = query.Where(a => a.Ano >= MinYear.Value);
                if (MaxYear.HasValue)
                    query = query.Where(a => a.Ano <= MaxYear.Value);

                // Apply sorting
                query = SortBy switch
                {
                    "price-asc" => query.OrderBy(a => a.Precio),
                    "price-desc" => query.OrderByDescending(a => a.Precio),
                    "year" => query.OrderByDescending(a => a.Ano),
                    _ => query.OrderByDescending(a => a.FechaCreacion) // "recent"
                };

                var totalCount = await query.CountAsync();
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

                Autos = await query.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();
            }
            catch
            {
                // Fallback to sample data if database fails
                var allAutos = GetSampleAutos();
                
                // Apply filters to sample data
                if (!string.IsNullOrEmpty(Brand))
                    allAutos = allAutos.Where(a => a.Marca.ToLower().Contains(Brand.ToLower())).ToList();
                if (!string.IsNullOrEmpty(Model))
                    allAutos = allAutos.Where(a => a.Modelo.ToLower().Contains(Model.ToLower())).ToList();
                if (MinPrice.HasValue)
                    allAutos = allAutos.Where(a => a.Precio >= MinPrice.Value).ToList();
                if (MaxPrice.HasValue)
                    allAutos = allAutos.Where(a => a.Precio <= MaxPrice.Value).ToList();
                if (MinYear.HasValue)
                    allAutos = allAutos.Where(a => a.Ano >= MinYear.Value).ToList();
                if (MaxYear.HasValue)
                    allAutos = allAutos.Where(a => a.Ano <= MaxYear.Value).ToList();

                // Apply sorting to sample data
                allAutos = SortBy switch
                {
                    "price-asc" => allAutos.OrderBy(a => a.Precio).ToList(),
                    "price-desc" => allAutos.OrderByDescending(a => a.Precio).ToList(),
                    "year" => allAutos.OrderByDescending(a => a.Ano).ToList(),
                    _ => allAutos.OrderByDescending(a => a.FechaCreacion).ToList() // "recent"
                };

                TotalPages = (int)Math.Ceiling((double)allAutos.Count / PageSize);
                Autos = allAutos.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            }
        }

        private List<Auto> GetSampleAutos()
        {
            return new List<Auto>
            {
                new Auto
                {
                    Id = 1,
                    Marca = "BMW",
                    Modelo = "X5M",
                    Ano = 2025,
                    Precio = 170000,
                    UbicacionExacta = "SUV deportivo de alto rendimiento",
                    ImagenPrincipal = "https://placehold.co/415x262",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Escazú",
                    PlacaVehiculo = "BMW001",
                    Condicion = "Excelente",
                    EmailPropietario = "bmw@dealer.com",
                    FechaCreacion = DateTime.Now.AddDays(-1)
                },
                new Auto
                {
                    Id = 2,
                    Marca = "Mercedes-Benz",
                    Modelo = "GLC 300 Coupé",
                    Ano = 2021,
                    Precio = 90000,
                    UbicacionExacta = "SUV coupé premium",
                    ImagenPrincipal = "https://placehold.co/415x262",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Santa Ana",
                    PlacaVehiculo = "MER002",
                    Condicion = "Excelente",
                    EmailPropietario = "mercedes@dealer.com",
                    FechaCreacion = DateTime.Now.AddDays(-2)
                },
                new Auto
                {
                    Id = 3,
                    Marca = "Audi",
                    Modelo = "Q5 Sportback",
                    Ano = 2022,
                    Precio = 75000,
                    UbicacionExacta = "SUV deportivo compacto",
                    ImagenPrincipal = "https://placehold.co/415x262",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "Alajuela",
                    Canton = "Alajuela",
                    PlacaVehiculo = "AUD003",
                    Condicion = "Muy Buena",
                    EmailPropietario = "audi@dealer.com",
                    FechaCreacion = DateTime.Now.AddDays(-3)
                }
            };
        }
    }
}
