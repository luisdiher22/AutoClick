using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages
{
    public class GuardadosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public GuardadosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Auto> SavedAutos { get; set; } = new List<Auto>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12; // 3 columns x 4 rows
        public string SortBy { get; set; } = "recent";

        public async Task OnGetAsync(int? page, string? sortBy)
        {
            CurrentPage = page ?? 1;
            SortBy = sortBy ?? "recent";

            // Try to get saved cars from database
            try
            {
                var query = _context.Autos.AsQueryable(); // Auto model doesn't have IsFavorite, using all for now
                
                // Apply sorting
                query = SortBy switch
                {
                    "price-asc" => query.OrderBy(a => a.ValorFiscal),
                    "price-desc" => query.OrderByDescending(a => a.ValorFiscal),
                    "year" => query.OrderByDescending(a => a.Ano),
                    _ => query.OrderByDescending(a => a.FechaCreacion) // "recent"
                };

                var totalCount = await query.CountAsync();
                TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);

                SavedAutos = await query
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch
            {
                // Fallback to sample data if database fails
                SavedAutos = GetSampleSavedAutos()
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                
                TotalPages = (int)Math.Ceiling((double)GetSampleSavedAutos().Count / PageSize);
            }
        }

        private List<Auto> GetSampleSavedAutos()
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
                    UbicacionExacta = "SUV deportivo de lujo con motor V8 biturbo",
                    ImagenPrincipal = "https://placehold.co/392x209",
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
                    Marca = "Audi",
                    Modelo = "Q5 Sportback",
                    Ano = 2022,
                    ValorFiscal = 75000,
                    UbicacionExacta = "SUV compacto deportivo con líneas dinámicas",
                    ImagenPrincipal = "https://placehold.co/392x209",
                    Carroceria = "SUV",
                    Combustible = "Gasolina",
                    Transmision = "Automática",
                    NumeroPuertas = 4,
                    Provincia = "Alajuela",
                    Canton = "Alajuela",
                    PlacaVehiculo = "AUD002",
                    Condicion = "Muy Buena",
                    EmailPropietario = "audi@dealer.com",
                    FechaCreacion = DateTime.Now.AddDays(-3)
                }
            };
        }
    }
}