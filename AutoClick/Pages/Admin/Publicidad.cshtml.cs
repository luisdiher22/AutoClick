using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Pages.Admin
{
    [Authorize]
    public class PublicidadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public PublicidadModel(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        // Listas de empresas
        public List<EmpresaPublicidad> EmpresasActivas { get; set; } = new();
        public List<EmpresaPublicidad> EmpresasInactivas { get; set; } = new();

        // Paginación
        [BindProperty(SupportsGet = true)]
        public int PaginaActualActivas { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PaginaActualInactivas { get; set; } = 1;

        public int TotalPaginasActivas { get; set; }
        public int TotalPaginasInactivas { get; set; }

        private const int TamañoPagina = 15;

        // Binding para formulario de nueva empresa
        [BindProperty]
        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreEmpresa { get; set; } = string.Empty;

        [BindProperty]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [BindProperty]
        [Required(ErrorMessage = "El estado de cobros es requerido")]
        public string EstadoCobros { get; set; } = "Al día";

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar autenticación
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Auth");
            }

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Auth");
            }

            // Verificar que es administrador
            var usuario = await _authService.GetUserByEmailAsync(userEmail!);
            if (usuario == null || !usuario.EsAdministrador)
            {
                return RedirectToPage("/Index");
            }

            await CargarDatosAsync();

            return Page();
        }

        private async Task CargarDatosAsync()
        {
            // Cargar todas las empresas con sus anuncios
            var todasLasEmpresas = await _context.EmpresasPublicidad
                .Include(e => e.Anuncios)
                .ToListAsync();

            // Filtrar empresas activas (que tienen al menos un anuncio activo O empresas nuevas sin anuncios)
            var empresasActivas = todasLasEmpresas
                .Where(e => e.Anuncios.Any(a => a.Activo) || !e.Anuncios.Any())
                .OrderByDescending(e => e.FechaInicio)
                .ToList();

            // Filtrar empresas inactivas (que tienen anuncios pero TODOS están inactivos)
            var empresasInactivas = todasLasEmpresas
                .Where(e => e.Anuncios.Any() && !e.Anuncios.Any(a => a.Activo))
                .OrderByDescending(e => e.FechaSalida ?? e.FechaInicio)
                .ToList();

            // Paginación para empresas activas
            var totalEmpresasActivas = empresasActivas.Count;
            TotalPaginasActivas = (int)Math.Ceiling(totalEmpresasActivas / (double)TamañoPagina);
            PaginaActualActivas = Math.Max(1, Math.Min(PaginaActualActivas, Math.Max(1, TotalPaginasActivas)));

            EmpresasActivas = empresasActivas
                .Skip((PaginaActualActivas - 1) * TamañoPagina)
                .Take(TamañoPagina)
                .ToList();

            // Paginación para empresas inactivas
            var totalEmpresasInactivas = empresasInactivas.Count;
            TotalPaginasInactivas = (int)Math.Ceiling(totalEmpresasInactivas / (double)TamañoPagina);
            PaginaActualInactivas = Math.Max(1, Math.Min(PaginaActualInactivas, Math.Max(1, TotalPaginasInactivas)));

            EmpresasInactivas = empresasInactivas
                .Skip((PaginaActualInactivas - 1) * TamañoPagina)
                .Take(TamañoPagina)
                .ToList();
        }

        public async Task<IActionResult> OnPostAgregarEmpresaAsync()
        {
            if (!ModelState.IsValid)
            {
                MensajeError = "Por favor complete todos los campos requeridos";
                await CargarDatosAsync();
                return Page();
            }

            try
            {
                // Verificar que no exista una empresa con el mismo nombre
                var empresaExistente = await _context.EmpresasPublicidad
                    .FirstOrDefaultAsync(e => e.NombreEmpresa == NombreEmpresa);

                if (empresaExistente != null)
                {
                    MensajeError = "Ya existe una empresa con ese nombre";
                    await CargarDatosAsync();
                    return Page();
                }

                var nuevaEmpresa = new EmpresaPublicidad
                {
                    NombreEmpresa = NombreEmpresa,
                    FechaInicio = DateTime.Now,
                    EstadoCobros = EstadoCobros,
                    Activa = true
                };

                _context.EmpresasPublicidad.Add(nuevaEmpresa);
                await _context.SaveChangesAsync();

                MensajeExito = "Empresa agregada exitosamente";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al agregar la empresa: {ex.Message}";
                await CargarDatosAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEliminarEmpresaAsync(int id)
        {
            try
            {
                var empresa = await _context.EmpresasPublicidad
                    .Include(e => e.Anuncios)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (empresa == null)
                {
                    MensajeError = "Empresa no encontrada";
                    return RedirectToPage();
                }

                // El cascade delete se encargará de eliminar los anuncios relacionados
                _context.EmpresasPublicidad.Remove(empresa);
                await _context.SaveChangesAsync();

                MensajeExito = $"Empresa '{empresa.NombreEmpresa}' y todos sus anuncios eliminados exitosamente";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al eliminar la empresa: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}
