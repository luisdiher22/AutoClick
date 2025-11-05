using AutoClick.Data;
using AutoClick.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoClick.ViewComponents
{
    public class AnuncioHorizontalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioHorizontalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.Horizontal)
                .OrderBy(x => Guid.NewGuid()) // Orden aleatorio
                .FirstOrDefaultAsync();

            if (anuncio != null)
            {
                // Incrementar vistas
                anuncio.NumeroVistas++;
                await _context.SaveChangesAsync();
            }

            return View(anuncio);
        }
    }

    public class AnuncioMedioVerticalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioMedioVerticalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.MedioVertical)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (anuncio != null)
            {
                anuncio.NumeroVistas++;
                await _context.SaveChangesAsync();
            }

            return View(anuncio);
        }
    }

    public class AnuncioGrandeVerticalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioGrandeVerticalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.GrandeVertical)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (anuncio != null)
            {
                anuncio.NumeroVistas++;
                await _context.SaveChangesAsync();
            }

            return View(anuncio);
        }
    }
}
