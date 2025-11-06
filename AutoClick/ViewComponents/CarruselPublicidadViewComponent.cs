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
            // formato: "cuadrado" o "horizontal"
            // seccion: nombre de la sección para filtrar anuncios

            // Mapear formato a TamanoAnuncio
            TamanoAnuncio tamanoFiltro;
            if (formato == "horizontal")
            {
                tamanoFiltro = TamanoAnuncio.Horizontal;
            }
            else if (formato == "medio-vertical")
            {
                tamanoFiltro = TamanoAnuncio.MedioVertical;
            }
            else // "cuadrado" o cualquier otro valor usa GrandeVertical
            {
                tamanoFiltro = TamanoAnuncio.GrandeVertical;
            }

            var anuncios = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.EmpresaPublicidad != null && a.Tamano == tamanoFiltro)
                .OrderByDescending(a => a.FechaPublicacion)
                .Take(10) // Máximo 10 anuncios activos
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
