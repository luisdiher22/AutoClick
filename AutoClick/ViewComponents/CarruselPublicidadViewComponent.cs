using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.ViewComponents
{
    public class CarruselPublicidadViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CarruselPublicidadViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string formato, string seccion)
        {
            // formato: "cuadrado", "horizontal", "medio-vertical"
            // seccion: nombre de la sección para filtrar anuncios

            // Mapear formato a MÚLTIPLES tamaños (desktop, tablet, móvil)
            List<TamanoAnuncio> tamanosPermitidos = formato switch
            {
                "horizontal" => new List<TamanoAnuncio> 
                { 
                    TamanoAnuncio.Horizontal,           // Desktop: 1010x189
                    TamanoAnuncio.TabletHorizontal,     // Tablet: 550x217
                    TamanoAnuncio.MobileHorizontal      // Móvil: 291x120
                },
                "medio-vertical" => new List<TamanoAnuncio> 
                { 
                    TamanoAnuncio.MedioVertical,        // Desktop: 401x287
                    TamanoAnuncio.TabletGrandeVertical, // Tablet: 340x373
                    TamanoAnuncio.MobileGrandeVertical  // Móvil: 291x180
                },
                _ => new List<TamanoAnuncio> // "cuadrado"
                { 
                    TamanoAnuncio.GrandeVertical,       // Desktop: 344x423
                    TamanoAnuncio.TabletGrandeVertical, // Tablet: 340x373
                    TamanoAnuncio.MobileGrandeVertical  // Móvil: 291x180
                }
            };

            // Consultar todos los anuncios de todos los tamaños permitidos
            var anuncios = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && 
                           a.EmpresaPublicidad != null && 
                           tamanosPermitidos.Contains(a.Tamano))
                .OrderByDescending(a => a.FechaPublicacion)
                .Take(10) // Máximo 10 anuncios activos por tamaño
                .ToListAsync();

            var viewModel = new CarruselPublicidadViewModel
            {
                Anuncios = anuncios,
                Formato = formato,
                Seccion = seccion
            };

            return View(viewModel);
        }
    }

    public class CarruselPublicidadViewModel
    {
        public List<AnuncioPublicidad> Anuncios { get; set; } = new();
        public string Formato { get; set; } = "cuadrado";
        public string Seccion { get; set; } = "";
    }
}
