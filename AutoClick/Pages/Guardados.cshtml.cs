using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoClick.Services;

namespace AutoClick.Pages
{
    [Authorize]
    public class GuardadosModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IBanderinesService _banderinesService;

        public GuardadosModel(ApplicationDbContext context, IBanderinesService banderinesService)
        {
            _context = context;
            _banderinesService = banderinesService;
        }

        public List<Auto> SavedAutos { get; set; } = new List<Auto>();
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
                    .Include(a => a.Propietario)
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
                
                // Calcular cantidad de autos por propietario
                foreach (var auto in SavedAutos)
                {
                    if (!string.IsNullOrEmpty(auto.EmailPropietario))
                    {
                        var count = await _context.Autos.CountAsync(a => a.EmailPropietario == auto.EmailPropietario && a.Activo);
                        AutoCountByOwner[auto.Id] = count;
                    }
                }
                
                // Cargar URLs de banderines
                await LoadBanderinUrlsAsync(SavedAutos);
            }
            catch
            {
                // Fallback to empty list if database fails
                SavedAutos = new List<Auto>();
                TotalPages = 0;
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
    }
}
