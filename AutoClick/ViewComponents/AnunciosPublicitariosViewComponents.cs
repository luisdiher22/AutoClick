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

    // Nuevos ViewComponents para Móvil
    public class AnuncioMobileHorizontalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioMobileHorizontalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.MobileHorizontal)
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

    public class AnuncioMobileGrandeVerticalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioMobileGrandeVerticalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.MobileGrandeVertical)
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

    // Nuevos ViewComponents para Tablet
    public class AnuncioTabletHorizontalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioTabletHorizontalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.TabletHorizontal)
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

    public class AnuncioTabletGrandeVerticalViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AnuncioTabletGrandeVerticalViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ubicacion = "general")
        {
            var anuncio = await _context.AnunciosPublicidad
                .Include(a => a.EmpresaPublicidad)
                .Where(a => a.Activo && a.Tamano == TamanoAnuncio.TabletGrandeVertical)
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
