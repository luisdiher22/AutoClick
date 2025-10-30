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
        private const int PageSize = 15;

        public PendientesAprobacionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PendingApprovalItem> PendingApprovals { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;
        
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public async Task OnGetAsync()
        {
            // Contar total de pendientes
            TotalRegistros = await _context.Autos
                .Where(a => a.PlanVisibilidad == 0 && a.Activo)
                .CountAsync();

            // Calcular total de páginas
            TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)PageSize);

            // Asegurar que la página actual esté en rango válido
            if (PaginaActual < 1) PaginaActual = 1;
            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;

            // Cargar anuncios pendientes de aprobación con paginación
            var pendingAutos = await _context.Autos
                .Where(a => a.PlanVisibilidad == 0 && a.Activo)
                .OrderByDescending(a => a.FechaCreacion)
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
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
