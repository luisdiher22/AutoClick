using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;

namespace AutoClick.Pages.Admin
{
    public class PublicacionesPagadasModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 15;

        public PublicacionesPagadasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PaidAdItem> PaidAds { get; set; } = new List<PaidAdItem>();
        
        [BindProperty(SupportsGet = true)]
        public int PaginaActual { get; set; } = 1;
        
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }

        public async Task OnGetAsync()
        {
            // Contar total de publicaciones pagadas
            TotalRegistros = await _context.Autos
                .Where(a => a.PlanVisibilidad > 0)
                .CountAsync();

            // Calcular total de páginas
            TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)PageSize);

            // Asegurar que la página actual esté en rango válido
            if (PaginaActual < 1) PaginaActual = 1;
            if (PaginaActual > TotalPaginas && TotalPaginas > 0) PaginaActual = TotalPaginas;

            // Obtener publicaciones pagadas con paginación
            PaidAds = await _context.Autos
                .Where(a => a.PlanVisibilidad > 0)
                .OrderByDescending(a => a.FechaCreacion) // Ordenar por fecha primero
                .ThenByDescending(a => a.PlanVisibilidad) // Luego por plan de visibilidad
                .Skip((PaginaActual - 1) * PageSize)
                .Take(PageSize)
                .Select(a => new PaidAdItem
                {
                    Id = a.Id,
                    Code = $"#{a.Id}",
                    Title = a.NombreCompleto,
                    Date = a.FechaCreacion,
                    PlanLevel = a.PlanVisibilidad,
                    Status = a.Activo ? "Activo" : "Inactivo"
                })
                .ToListAsync();
        }

        public class PaidAdItem
        {
            public int Id { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public int PlanLevel { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
