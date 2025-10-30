using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoClick.Pages.Admin
{
    public class HistorialRechazosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistorialRechazosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<RejectedAdItem> RejectedAds { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Cargar anuncios rechazados (Activo = false y PlanVisibilidad = 0)
            var rejectedAutos = await _context.Autos
                .Where(a => !a.Activo && a.PlanVisibilidad == 0)
                .OrderByDescending(a => a.FechaActualizacion)
                .Select(a => new RejectedAdItem
                {
                    Id = a.Id,
                    Code = $"#{a.Id}",
                    Title = a.NombreCompleto,
                    Status = "Rechazado"
                })
                .ToListAsync();

            RejectedAds = rejectedAutos;
        }
    }

    public class RejectedAdItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
