using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AutoClick.Pages
{
    [Authorize]
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

            // Obtener el email del usuario autenticado
            var emailUsuario = User.Identity?.IsAuthenticated == true 
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value 
                : null;

            // Si el usuario no está autenticado, no mostrar ningún auto
            if (string.IsNullOrEmpty(emailUsuario))
            {
                SavedAutos = new List<Auto>();
                TotalPages = 0;
                return;
            }

            try
            {
                // Obtener los IDs de los autos marcados como favoritos por este usuario
                var favoritosIds = await _context.Favoritos
                    .Where(f => f.EmailUsuario == emailUsuario)
                    .Select(f => f.AutoId)
                    .ToListAsync();

                // Si no tiene favoritos, retornar lista vacía
                if (!favoritosIds.Any())
                {
                    SavedAutos = new List<Auto>();
                    TotalPages = 0;
                    return;
                }

                // Obtener los autos favoritos
                var query = _context.Autos
                    .Where(a => favoritosIds.Contains(a.Id) && a.Activo);
                
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

                SavedAutos = await query
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch
            {
                // Fallback to empty list if database fails
                SavedAutos = new List<Auto>();
                TotalPages = 0;
            }
        }
    }
}
