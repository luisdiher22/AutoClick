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

        public CentroAnunciosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Auto> Anuncios { get; set; } = new();

        public async Task OnGetAsync()
        {
            Anuncios = await _context.Autos
                .OrderByDescending(a => a.FechaCreacion)
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
