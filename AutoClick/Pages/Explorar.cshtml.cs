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
                // Si hay error de base de datos, mostrar lista vacía
                Autos = new List<Auto>();
                TotalPages = 1;
            }
        }
    }
}
