using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.Pages.Admin
{
    public class CentroAnunciosModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 15;

        public CentroAnunciosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Auto> Anuncios { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public async Task OnGetAsync()
        {
            TotalRegistros = await _context.Autos.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)TotalRegistros / PageSize);
            
            // Asegurar que la página actual sea válida
            if (PaginaActual < 1) PaginaActual = 1;
            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;
            
            Anuncios = await _context.Autos
                .OrderByDescending(a => a.FechaCreacion)
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetDetallesAsync(int id)
        {
            var anuncio = await _context.Autos.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return new JsonResult(anuncio);
        }

        public async Task<IActionResult> OnPostActualizarValorFiscalAsync(int id, decimal nuevoValorFiscal)
        {
            var anuncio = await _context.Autos.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }

            anuncio.ValorFiscal = nuevoValorFiscal;
            anuncio.FechaActualizacion = DateTime.Now;
            
            _context.Autos.Update(anuncio);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Valor Fiscal actualizado correctamente" });
        }
    }
}
