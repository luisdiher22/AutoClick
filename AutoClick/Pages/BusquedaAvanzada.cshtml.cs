using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class BusquedaAvanzadaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BusquedaAvanzadaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Auto> Autos { get; set; } = new List<Auto>();
        
        // Filter Parameters
        [BindProperty(SupportsGet = true)]
        public string? Province { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Brand { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Model { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinPrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxPrice { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinKm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxKm { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MinYear { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? MaxYear { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? BodyType { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? FuelType { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Transmission { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Condition { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "recent";
        
        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;
        
        public int PageSize { get; set; } = 8; // 2 columns x 4 rows
        public int TotalPages { get; set; }
        public int CurrentPage => Page;
        public int TotalCars { get; set; }

        // Applied Filters for Display
        public List<string> AppliedFilters { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            var query = _context.Autos.AsQueryable();

            // Build applied filters list for UI
            BuildAppliedFiltersList();

            // Apply filters
            if (!string.IsNullOrEmpty(Search))
                query = query.Where(a => a.Marca.Contains(Search) || a.Modelo.Contains(Search));
                
            if (!string.IsNullOrEmpty(Province))
                query = query.Where(a => a.Provincia.Contains(Province));
                
            if (!string.IsNullOrEmpty(Canton))
                query = query.Where(a => a.Canton.Contains(Canton));
                
            if (!string.IsNullOrEmpty(Brand))
                query = query.Where(a => a.Marca == Brand);
                
            if (!string.IsNullOrEmpty(Model))
                query = query.Where(a => a.Modelo == Model);
                
            if (MinPrice.HasValue)
                query = query.Where(a => a.ValorFiscal >= MinPrice.Value);
                
            if (MaxPrice.HasValue)
                query = query.Where(a => a.ValorFiscal <= MaxPrice.Value);
                
            if (MinYear.HasValue)
                query = query.Where(a => a.Ano >= MinYear.Value);
                
            if (MaxYear.HasValue)
                query = query.Where(a => a.Ano <= MaxYear.Value);

            // Apply sorting
            query = SortBy switch
            {
                "price-asc" => query.OrderBy(a => a.ValorFiscal),
                "price-desc" => query.OrderByDescending(a => a.ValorFiscal),
                "year" => query.OrderByDescending(a => a.Ano),
                "brand" => query.OrderBy(a => a.Marca).ThenBy(a => a.Modelo),
                _ => query.OrderByDescending(a => a.Id) // Default: most recent
            };

            // Count total cars for pagination
            TotalCars = await query.CountAsync();
            TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);

            // Apply pagination
            Autos = await query
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // If no autos in database, use sample data
            if (!Autos.Any())
            {
                var sampleAutos = GetSampleSearchResults();
                
                // Apply filters to sample data
                sampleAutos = ApplyFiltersToSampleData(sampleAutos);
                
                TotalCars = sampleAutos.Count;
                TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);
                
                Autos = sampleAutos
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
        }

        private void BuildAppliedFiltersList()
        {
            AppliedFilters.Clear();
            
            if (!string.IsNullOrEmpty(Province))
                AppliedFilters.Add(Province);
                
            if (!string.IsNullOrEmpty(Canton))
                AppliedFilters.Add(Canton);
                
            if (!string.IsNullOrEmpty(Brand))
                AppliedFilters.Add(Brand);
                
            if (!string.IsNullOrEmpty(Model))
                AppliedFilters.Add(Model);
                
            if (MinPrice.HasValue || MaxPrice.HasValue)
            {
                var priceRange = "";
                if (MinPrice.HasValue) priceRange += $"${MinPrice:N0}";
                if (MinPrice.HasValue && MaxPrice.HasValue) priceRange += " - ";
                if (MaxPrice.HasValue) priceRange += $"${MaxPrice:N0}";
                if (!string.IsNullOrEmpty(priceRange))
                    AppliedFilters.Add($"Precio: {priceRange}");
            }
        }

        private List<Auto> ApplyFiltersToSampleData(List<Auto> autos)
        {
            if (!string.IsNullOrEmpty(Brand))
                autos = autos.Where(a => a.Marca == Brand).ToList();
                
            if (!string.IsNullOrEmpty(Model))
                autos = autos.Where(a => a.Modelo == Model).ToList();
                
            if (MinPrice.HasValue)
                autos = autos.Where(a => a.ValorFiscal >= MinPrice.Value).ToList();
                
            if (MaxPrice.HasValue)
                autos = autos.Where(a => a.ValorFiscal <= MaxPrice.Value).ToList();
                
            if (MinYear.HasValue)
                autos = autos.Where(a => a.Ano >= MinYear.Value).ToList();
                
            if (MaxYear.HasValue)
                autos = autos.Where(a => a.Ano <= MaxYear.Value).ToList();

            // Apply sorting
            autos = SortBy switch
            {
                "price-asc" => autos.OrderBy(a => a.ValorFiscal).ToList(),
                "price-desc" => autos.OrderByDescending(a => a.ValorFiscal).ToList(),
                "year" => autos.OrderByDescending(a => a.Ano).ToList(),
                "brand" => autos.OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToList(),
                _ => autos.OrderByDescending(a => a.Id).ToList()
            };

            return autos;
        }

        private List<Auto> GetSampleSearchResults()
        {
            return new List<Auto>
            {
                new Auto
                {
                    Id = 1,
                    Marca = "BMW",
                    Modelo = "X5M",
                    Ano = 2025,
                    ValorFiscal = 170000,
                    ImagenPrincipal = "https://placehold.co/415x262/333333/FFFFFF?text=BMW+X5M",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Escazú",
                    PlacaVehiculo = "BMW001",
                    Condicion = "Excelente",
                    EmailPropietario = "bmw@dealer.com"
                },
                new Auto
                {
                    Id = 2,
                    Marca = "Audi",
                    Modelo = "Q5 Sportback",
                    Ano = 2022,
                    ValorFiscal = 75000,
                    ImagenPrincipal = "https://placehold.co/415x262/8B0000/FFFFFF?text=Audi+Q5",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "Alajuela",
                    Canton = "Alajuela",
                    PlacaVehiculo = "AUD002",
                    Condicion = "Muy Buena",
                    EmailPropietario = "audi@dealer.com"
                },
                new Auto
                {
                    Id = 3,
                    Marca = "Mercedes-Benz",
                    Modelo = "GLE 450",
                    Ano = 2023,
                    ValorFiscal = 85000,
                    ImagenPrincipal = "https://placehold.co/415x262/000000/FFFFFF?text=Mercedes+GLE",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Santa Ana",
                    PlacaVehiculo = "MER003",
                    Condicion = "Excelente",
                    EmailPropietario = "mercedes@dealer.com"
                }
            };
        }
    }
}