using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Models;
using AutoClick.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AutoClick.Pages
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class MisAnunciosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MisAnunciosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Filter Properties
        [BindProperty(SupportsGet = true)]
        public string? Provincia { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Canton { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Marca { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Modelo { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Precio mínimo")]
        public decimal? PrecioMin { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Precio máximo")]
        public decimal? PrecioMax { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Kilometraje mínimo")]
        public int? KilometrajeMin { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Kilometraje máximo")]
        public int? KilometrajeMax { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Año mínimo")]
        public int? AñoMin { get; set; }

        [BindProperty(SupportsGet = true)]
        [Display(Name = "Año máximo")]
        public int? AñoMax { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Carroceria { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Combustible { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Traccion { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TipoVendedor { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Condicion { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Transmision { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Pasajeros { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Extras { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrdenarPor { get; set; } = "recientes";

        // Pagination Properties
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;

        public int TotalPaginas { get; set; }
        public int TotalAnuncios { get; set; }
        public const int AnunciosPorPagina = 12;

        // Data Properties
        public List<AnuncioViewModel> Anuncios { get; set; } = new List<AnuncioViewModel>();
        public List<string> AppliedFilters { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Auth");
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Auth");
            }
            
            BuildAppliedFiltersList();
            await LoadAnunciosAsync(userEmail);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {

            
            // Check if user is logged in (same authentication as OnGet)
            if (!User.Identity?.IsAuthenticated == true)
            {

                return new JsonResult(new { success = false, message = "Usuario no autenticado" });
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                ?? User.FindFirst("email")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                

                
            if (string.IsNullOrEmpty(userEmail))
            {

                return new JsonResult(new { success = false, message = "Usuario no válido" });
            }

            try
            {

                
                // Find the auto and verify ownership
                var auto = await _context.Autos.FirstOrDefaultAsync(a => a.Id == id && a.EmailPropietario == userEmail);
                
                if (auto == null)
                {

                    return new JsonResult(new { success = false, message = "Anuncio no encontrado o sin permisos" });
                }
                


                // Delete the auto

                _context.Autos.Remove(auto);
                await _context.SaveChangesAsync();
                

                return new JsonResult(new { success = true, message = "Anuncio eliminado exitosamente" });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { success = false, message = "Error al eliminar el anuncio" });
            }
        }

        private async Task LoadAnunciosAsync(string userEmail)
        {
            try
            {
                // Get the current user
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (usuario == null)
                {
                    Anuncios = new List<AnuncioViewModel>();
                    TotalAnuncios = 0;
                    TotalPaginas = 0;
                    return;
                }

                // Filter autos by current user's email
                var query = _context.Autos.Where(a => a.EmailPropietario == userEmail && a.Activo);

                // Apply filters
                query = ApplyFilters(query);

                // Get total count for pagination
                TotalAnuncios = await query.CountAsync();
                TotalPaginas = (int)Math.Ceiling((double)TotalAnuncios / AnunciosPorPagina);

                // Apply sorting
                query = ApplySorting(query);

                // Apply pagination
                var skip = (PaginaActual - 1) * AnunciosPorPagina;
                var autos = await query.AsNoTracking().Skip(skip).Take(AnunciosPorPagina).ToListAsync();

                // Convert to view model with more realistic data
                Anuncios = autos.Select(auto => new AnuncioViewModel
                {
                    Id = auto.Id,
                    Titulo = auto.NombreCompleto,
                    Precio = auto.Precio,
                    ImagenPrincipal = !string.IsNullOrEmpty(auto.ImagenPrincipal) ? auto.ImagenPrincipal : 
                                     (auto.ImagenesUrlsList.Any() ? auto.ImagenesUrlsList.First() : "https://via.placeholder.com/416x262"),
                    Vistas = Random.Shared.Next(50, 500), // TODO: Implement real view tracking
                    MeGusta = Random.Shared.Next(5, 50), // TODO: Implement real like system
                    FechaPublicacion = auto.FechaCreacion,
                    DiasRestantes = CalcularDiasRestantes(auto.FechaCreacion, auto.PlanVisibilidad),
                    Estado = DeterminarEstadoAnuncio(auto),
                    EsDestacado = auto.PlanVisibilidad >= 3 || auto.BanderinAdquirido > 0
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log error and show empty results

                Anuncios = new List<AnuncioViewModel>();
                TotalAnuncios = 0;
                TotalPaginas = 0;
            }
        }

        private IQueryable<Auto> ApplyFilters(IQueryable<Auto> query)
        {
            if (!string.IsNullOrEmpty(Provincia))
            {
                query = query.Where(c => c.Provincia != null && c.Provincia.Contains(Provincia));
            }

            if (!string.IsNullOrEmpty(Canton))
            {
                query = query.Where(c => c.Canton != null && c.Canton.Contains(Canton));
            }

            if (!string.IsNullOrEmpty(Marca))
            {
                query = query.Where(a => a.Marca == Marca);
            }

            if (!string.IsNullOrEmpty(Modelo))
            {
                query = query.Where(a => a.Modelo == Modelo);
            }

            if (PrecioMin.HasValue)
            {
                query = query.Where(a => a.Precio >= PrecioMin.Value);
            }

            if (PrecioMax.HasValue)
            {
                query = query.Where(a => a.Precio <= PrecioMax.Value);
            }

            if (KilometrajeMin.HasValue)
            {
                query = query.Where(c => c.Kilometraje >= KilometrajeMin.Value);
            }

            if (KilometrajeMax.HasValue)
            {
                query = query.Where(c => c.Kilometraje <= KilometrajeMax.Value);
            }

            if (AñoMin.HasValue)
            {
                query = query.Where(a => a.Ano >= AñoMin.Value);
            }

            if (AñoMax.HasValue)
            {
                query = query.Where(a => a.Ano <= AñoMax.Value);
            }

            if (!string.IsNullOrEmpty(Carroceria))
            {
                query = query.Where(a => a.Carroceria == Carroceria);
            }

            if (!string.IsNullOrEmpty(Combustible))
            {
                query = query.Where(c => c.Combustible == Combustible);
            }

            if (!string.IsNullOrEmpty(Traccion))
            {
                query = query.Where(c => c.Traccion == Traccion);
            }

            if (!string.IsNullOrEmpty(Condicion))
            {
                query = query.Where(c => c.Condicion == Condicion);
            }

            if (!string.IsNullOrEmpty(Transmision))
            {
                query = query.Where(c => c.Transmision == Transmision);
            }

            if (!string.IsNullOrEmpty(Pasajeros))
            {
                if (int.TryParse(Pasajeros.Replace("+", ""), out int passengerCount))
                {
                    if (Pasajeros.Contains("+"))
                    {
                        query = query.Where(c => c.NumeroPasajeros >= passengerCount);
                    }
                    else
                    {
                        query = query.Where(c => c.NumeroPasajeros == passengerCount);
                    }
                }
            }

            return query;
        }

        private IQueryable<Auto> ApplySorting(IQueryable<Auto> query)
        {
            return OrdenarPor switch
            {
                "antiguos" => query.OrderBy(c => c.FechaCreacion),
                "precio-asc" => query.OrderBy(a => a.Precio),
                "precio-desc" => query.OrderByDescending(a => a.Precio),
                "kilometraje" => query.OrderBy(c => c.Kilometraje),
                _ => query.OrderByDescending(c => c.FechaCreacion) // Default: most recent
            };
        }

        private int CalcularDiasRestantes(DateTime fechaPublicacion, int planVisibilidad)
        {
            // Different plans have different durations
            var diasDuracion = planVisibilidad switch
            {
                1 => 30,  // Plan básico: 30 días
                2 => 45,  // Plan estándar: 45 días
                3 => 60,  // Plan premium: 60 días
                4 => 90,  // Plan empresarial: 90 días
                5 => 180, // Plan corporativo: 6 meses
                _ => 30
            };

            var fechaExpiracion = fechaPublicacion.AddDays(diasDuracion);
            var diasRestantes = (fechaExpiracion - DateTime.Now).Days;
            return Math.Max(0, diasRestantes);
        }

        private EstadoAnuncio DeterminarEstadoAnuncio(Auto auto)
        {
            var diasRestantes = CalcularDiasRestantes(auto.FechaCreacion, auto.PlanVisibilidad);
            
            if (!auto.Activo)
                return EstadoAnuncio.Pausado;
            
            if (diasRestantes <= 0)
                return EstadoAnuncio.Expirado;
            
            return EstadoAnuncio.Activo;
        }

        public async Task<IActionResult> OnPostAsync()
        {

            
            // Check if this is a delete request
            if (Request.Form.ContainsKey("action") && Request.Form["action"] == "delete")
            {
                if (int.TryParse(Request.Form["id"], out int id))
                {

                    return await OnPostDeleteAsync(id);
                }
            }
            
            return Page();
        }

        private void BuildAppliedFiltersList()
        {
            AppliedFilters.Clear();
            
            if (!string.IsNullOrEmpty(Provincia))
                AppliedFilters.Add(Provincia);
                
            if (!string.IsNullOrEmpty(Canton))
                AppliedFilters.Add(Canton);
                
            if (!string.IsNullOrEmpty(Marca))
                AppliedFilters.Add(Marca);
                
            if (!string.IsNullOrEmpty(Modelo))
                AppliedFilters.Add(Modelo);
                
            if (!string.IsNullOrEmpty(Carroceria))
                AppliedFilters.Add(Carroceria);
                
            if (!string.IsNullOrEmpty(Combustible))
                AppliedFilters.Add(Combustible);
                
            if (!string.IsNullOrEmpty(Traccion))
                AppliedFilters.Add(Traccion);
                
            if (!string.IsNullOrEmpty(TipoVendedor))
                AppliedFilters.Add(TipoVendedor);
                
            if (!string.IsNullOrEmpty(Condicion))
                AppliedFilters.Add(Condicion);
                
            if (!string.IsNullOrEmpty(Transmision))
                AppliedFilters.Add(Transmision);
                
            if (!string.IsNullOrEmpty(Pasajeros))
                AppliedFilters.Add(Pasajeros);
                
            if (!string.IsNullOrEmpty(Extras))
                AppliedFilters.Add(Extras);
                
            if (PrecioMin.HasValue || PrecioMax.HasValue)
            {
                var priceRange = "";
                if (PrecioMin.HasValue) priceRange += $"₡{PrecioMin:N0}";
                if (PrecioMin.HasValue && PrecioMax.HasValue) priceRange += " - ";
                if (PrecioMax.HasValue) priceRange += $"₡{PrecioMax:N0}";
                if (!string.IsNullOrEmpty(priceRange))
                    AppliedFilters.Add(priceRange);
            }
            
            if (KilometrajeMin.HasValue || KilometrajeMax.HasValue)
            {
                var kmRange = "";
                if (KilometrajeMin.HasValue) kmRange += $"{KilometrajeMin:N0} km";
                if (KilometrajeMin.HasValue && KilometrajeMax.HasValue) kmRange += " - ";
                if (KilometrajeMax.HasValue) kmRange += $"{KilometrajeMax:N0} km";
                if (!string.IsNullOrEmpty(kmRange))
                    AppliedFilters.Add(kmRange);
            }
            
            if (AñoMin.HasValue || AñoMax.HasValue)
            {
                var yearRange = "";
                if (AñoMin.HasValue) yearRange += $"{AñoMin}";
                if (AñoMin.HasValue && AñoMax.HasValue) yearRange += " - ";
                if (AñoMax.HasValue) yearRange += $"{AñoMax}";
                if (!string.IsNullOrEmpty(yearRange))
                    AppliedFilters.Add(yearRange);
            }
        }
    }

    // View Model for displaying listings
    public class AnuncioViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? ImagenPrincipal { get; set; }
        public int Vistas { get; set; }
        public int MeGusta { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int DiasRestantes { get; set; }
        public EstadoAnuncio Estado { get; set; }
        public bool EsDestacado { get; set; }
    }

    // Enum for listing status
    public enum EstadoAnuncio
    {
        Activo,
        Pausado,
        Expirado
    }
}
