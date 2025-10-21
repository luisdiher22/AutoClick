using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class DestacadosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DestacadosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Auto> Autos { get; set; } = new List<Auto>();
        
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "recent";
        
        [BindProperty(SupportsGet = true)]
        public new int Page { get; set; } = 1;
        
        public int PageSize { get; set; } = 12; // 3x4 grid for featured cars
        public int TotalPages { get; set; }
        public int CurrentPage => Page;
        public int TotalCars { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Solo mostrar autos destacados (con plan de visibilidad > 1) y activos
                var query = _context.Autos
                    .Where(a => a.Activo && a.PlanVisibilidad > 1);

                // Apply sorting
                query = SortBy switch
                {
                    "price-asc" => query.OrderBy(a => a.Precio),
                    "price-desc" => query.OrderByDescending(a => a.Precio),
                    "year" => query.OrderByDescending(a => a.Ano),
                    "brand" => query.OrderBy(a => a.Marca).ThenBy(a => a.Modelo),
                    _ => query.OrderByDescending(a => a.PlanVisibilidad).ThenByDescending(a => a.FechaCreacion) // Featured first, then recent
                };

                // Count total cars for pagination
                TotalCars = await query.CountAsync();
                TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);

                // Apply pagination
                Autos = await query
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                // If no featured autos in database, use sample data
                if (!Autos.Any())
                {
                    var sampleData = GetSampleFeaturedAutos();
                    Autos = sampleData
                        .Skip((Page - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();
                        
                    TotalCars = sampleData.Count;
                    TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);
                }
            }
            catch (Exception)
            {
                // Fallback to sample data on any error
                var sampleData = GetSampleFeaturedAutos();
                Autos = sampleData
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                    
                TotalCars = sampleData.Count;
                TotalPages = (int)Math.Ceiling((double)TotalCars / PageSize);
            }
        }

        private List<Auto> GetSampleFeaturedAutos()
        {
            var sampleAutos = new List<Auto>
            {
                new Auto
                {
                    Id = 1,
                    Marca = "Mercedes-Benz",
                    Modelo = "Clase S",
                    Ano = 2024,
                    Precio = 85000,
                    ImagenPrincipal = "https://placehold.co/400x300/0066CC/FFFFFF?text=Mercedes+S-Class",
                    Carroceria = "Sedán",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Escazú",
                    PlacaVehiculo = "MER001",
                    Condicion = "Excelente",
                    EmailPropietario = "mercedes@dealer.com",
                    PlanVisibilidad = 3, // Destacado premium
                    FechaCreacion = DateTime.Now.AddDays(-1),
                    Activo = true
                },
                new Auto
                {
                    Id = 2,
                    Marca = "BMW",
                    Modelo = "X7",
                    Ano = 2023,
                    Precio = 92000,
                    ImagenPrincipal = "https://placehold.co/400x300/333333/FFFFFF?text=BMW+X7",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "San José",
                    Canton = "Santa Ana",
                    PlacaVehiculo = "BMW002",
                    Condicion = "Excelente",
                    EmailPropietario = "bmw@dealer.com",
                    PlanVisibilidad = 2, // Destacado estándar
                    FechaCreacion = DateTime.Now.AddDays(-2),
                    Activo = true
                }
            };

            // Apply sorting to sample data
            return SortBy switch
            {
                "price-asc" => sampleAutos.OrderBy(a => a.Precio).ToList(),
                "price-desc" => sampleAutos.OrderByDescending(a => a.Precio).ToList(),
                "year" => sampleAutos.OrderByDescending(a => a.Ano).ToList(),
                "brand" => sampleAutos.OrderBy(a => a.Marca).ThenBy(a => a.Modelo).ToList(),
                _ => sampleAutos.OrderByDescending(a => a.PlanVisibilidad).ThenByDescending(a => a.FechaCreacion).ToList()
            };
        }
    }
}
