using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoClick.Pages.Admin
{
    public class PendientesAprobacionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PendientesAprobacionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PendingApprovalItem> PendingApprovals { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Cargar anuncios pendientes de aprobación (PlanVisibilidad = 0 y Activo = true)
            var pendingAutos = await _context.Autos
                .Where(a => a.PlanVisibilidad == 0 && a.Activo)
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new PendingApprovalItem
                {
                    Id = a.Id,
                    Code = $"#{a.Id}",
                    Title = a.NombreCompleto,
                    Date = a.FechaCreacion.ToString("dd/MM/yyyy")
                })
                .ToListAsync();

            PendingApprovals = pendingAutos;
        }
    }

    public class PendingApprovalItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
