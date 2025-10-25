using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Services;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAutoService _autoService;
    private readonly IBanderinesService _banderinesService;
    private readonly ApplicationDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, IAutoService autoService, IBanderinesService banderinesService, ApplicationDbContext context)
    {
        _logger = logger;
        _autoService = autoService;
        _banderinesService = banderinesService;
        _context = context;
    }

    public List<Auto> AutosDestacados { get; set; } = new();
    public List<Auto> AutosRecientes { get; set; } = new();
    public List<Auto> AutosGuardados { get; set; } = new();
    public List<Auto> AutosExploracion { get; set; } = new();
    public Dictionary<int, string?> BanderinUrls { get; set; } = new();

    public async Task<string?> GetBanderinUrlAsync(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;
            
        return await _banderinesService.GetBanderinUrlAsync(fileName);
    }

    private async Task LoadBanderinUrlsAsync(List<Auto> autos)
    {
        _logger.LogInformation("=== LoadBanderinUrlsAsync START ===");
        _logger.LogInformation($"Processing {autos.Count} autos");
        
        foreach (var auto in autos)
        {
            _logger.LogInformation($"Auto {auto.Id}: BanderinAdquirido={auto.BanderinAdquirido}, BanderinVideoUrl={auto.BanderinVideoUrl}");
            
            if (!string.IsNullOrEmpty(auto.BanderinVideoUrl) && !BanderinUrls.ContainsKey(auto.Id))
            {
                _logger.LogInformation($"Attempting to load banderin URL for auto {auto.Id}: {auto.BanderinVideoUrl}");
                var url = await _banderinesService.GetBanderinUrlAsync(auto.BanderinVideoUrl);
                _logger.LogInformation($"Result URL for auto {auto.Id}: {url}");
                BanderinUrls[auto.Id] = url;
            }
            else
            {
                _logger.LogWarning($"Skipping auto {auto.Id}: BanderinVideoUrl is null or already loaded");
            }
        }
        
        _logger.LogInformation($"=== LoadBanderinUrlsAsync END: Loaded {BanderinUrls.Count} URLs ===");
    }

    public async Task OnGetAsync()
    {
        try
        {
            // Obtener autos destacados usando el servicio (solo con PlanVisibilidad > 1)
            AutosDestacados = await _autoService.GetAutosDestacadosAsync(3);

            // Obtener autos más recientes
            AutosRecientes = await _autoService.GetAutosRecientesAsync(3);

            // Obtener autos guardados (favoritos) del usuario autenticado
            var emailUsuario = User.Identity?.IsAuthenticated == true 
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("Email")?.Value 
                : null;

            if (!string.IsNullOrEmpty(emailUsuario))
            {
                // Obtener los IDs de los autos favoritos del usuario
                var favoritosIds = await _context.Favoritos
                    .Where(f => f.EmailUsuario == emailUsuario)
                    .Select(f => f.AutoId)
                    .ToListAsync();

                if (favoritosIds.Any())
                {
                    // Cargar los autos favoritos
                    AutosGuardados = await _context.Autos
                        .Where(a => favoritosIds.Contains(a.Id) && a.Activo)
                        .OrderByDescending(a => a.FechaCreacion)
                        .Take(3)
                        .ToListAsync();
                }
                else
                {
                    AutosGuardados = new List<Auto>();
                }
            }
            else
            {
                AutosGuardados = new List<Auto>();
            }

            // Para exploración general
            AutosExploracion = await _autoService.GetAutosAleatoriosAsync(3);

            // Cargar URLs de banderines para todos los autos
            var allAutos = new List<Auto>();
            allAutos.AddRange(AutosDestacados);
            allAutos.AddRange(AutosRecientes);
            allAutos.AddRange(AutosGuardados);
            allAutos.AddRange(AutosExploracion);
            
            await LoadBanderinUrlsAsync(allAutos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar autos en la página principal");
            
            // En caso de error, mantener las listas vacías
            AutosDestacados = new List<Auto>();
            AutosRecientes = new List<Auto>();
            AutosGuardados = new List<Auto>();
            AutosExploracion = new List<Auto>();
        }
    }
}
