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
    public class DetallePublicidadEmpresaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IPublicidadStorageService _publicidadStorageService;

        public DetallePublicidadEmpresaModel(
            ApplicationDbContext context, 
            IAuthService authService,
            IPublicidadStorageService publicidadStorageService)
        {
            _context = context;
            _authService = authService;
            _publicidadStorageService = publicidadStorageService;
        }

        // Información de la empresa
        public EmpresaPublicidad Empresa { get; set; } = new();

        // Listas de anuncios
        public List<AnuncioPublicidad> AnunciosActivos { get; set; } = new();
        public List<AnuncioPublicidad> AnunciosInactivos { get; set; } = new();

        // Binding para formulario de nuevo anuncio
        [BindProperty]
        public IFormFile? ImagenAnuncio { get; set; }

        [BindProperty]
        public int EmpresaId { get; set; }

        [BindProperty]
        public int? AnuncioId { get; set; }

        [BindProperty]
        [StringLength(500, ErrorMessage = "La URL no puede exceder 500 caracteres")]
        public string UrlImagen { get; set; } = string.Empty;

        [BindProperty]
        [StringLength(500, ErrorMessage = "La URL de destino no puede exceder 500 caracteres")]
        public string? UrlDestino { get; set; }

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
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

            await CargarDatosAsync(id);

            if (Empresa.Id == 0)
            {
                MensajeError = "Empresa no encontrada";
                return RedirectToPage("/Admin/Publicidad");
            }

            return Page();
        }

        private async Task CargarDatosAsync(int empresaId)
        {
            var empresa = await _context.EmpresasPublicidad
                .Include(e => e.Anuncios)
                .FirstOrDefaultAsync(e => e.Id == empresaId);

            if (empresa == null)
            {
                return;
            }

            Empresa = empresa;

            // Filtrar anuncios activos e inactivos
            AnunciosActivos = empresa.Anuncios
                .Where(a => a.Activo)
                .OrderByDescending(a => a.FechaPublicacion)
                .ToList();

            AnunciosInactivos = empresa.Anuncios
                .Where(a => !a.Activo)
                .OrderByDescending(a => a.FechaPublicacion)
                .ToList();
        }

        public async Task<IActionResult> OnPostAgregarAnuncioAsync()
        {
            try
            {
                var empresa = await _context.EmpresasPublicidad.FindAsync(EmpresaId);
                if (empresa == null)
                {
                    MensajeError = "Empresa no encontrada";
                    return RedirectToPage("/Admin/Publicidad");
                }

                string urlImagen;

                // Si se subió un archivo, usar el servicio de almacenamiento
                if (ImagenAnuncio != null && ImagenAnuncio.Length > 0)
                {
                    try
                    {
                        urlImagen = await _publicidadStorageService.UploadAnuncioImageAsync(ImagenAnuncio);
                    }
                    catch (ArgumentException ex)
                    {
                        MensajeError = $"Error en la validación de la imagen: {ex.Message}";
                        return RedirectToPage(new { id = EmpresaId });
                    }
                }
                // Si no se subió archivo, usar la URL proporcionada manualmente
                else if (!string.IsNullOrWhiteSpace(UrlImagen))
                {
                    urlImagen = UrlImagen;
                }
                else
                {
                    MensajeError = "Debe proporcionar una imagen o una URL de imagen";
                    return RedirectToPage(new { id = EmpresaId });
                }

                var nuevoAnuncio = new AnuncioPublicidad
                {
                    EmpresaPublicidadId = EmpresaId,
                    UrlImagen = urlImagen,
                    UrlDestino = string.IsNullOrWhiteSpace(UrlDestino) ? null : UrlDestino,
                    FechaPublicacion = DateTime.Now,
                    NumeroVistas = 0,
                    NumeroClics = 0,
                    Activo = true
                };

                _context.AnunciosPublicidad.Add(nuevoAnuncio);
                await _context.SaveChangesAsync();

                MensajeExito = "Anuncio agregado exitosamente";
                return RedirectToPage(new { id = EmpresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al agregar el anuncio: {ex.Message}";
                return RedirectToPage(new { id = EmpresaId });
            }
        }

        public async Task<IActionResult> OnPostEditarAnuncioAsync()
        {
            if (!AnuncioId.HasValue)
            {
                MensajeError = "Datos inválidos para editar el anuncio";
                return RedirectToPage(new { id = EmpresaId });
            }

            try
            {
                // Usar AsTracking() para forzar el seguimiento de cambios
                var anuncio = await _context.AnunciosPublicidad
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == AnuncioId.Value);
                    
                if (anuncio == null)
                {
                    MensajeError = "Anuncio no encontrado";
                    return RedirectToPage(new { id = EmpresaId });
                }

                string nuevaUrlImagen = anuncio.UrlImagen; // Mantener la imagen actual por defecto

                // Si se subió un archivo nuevo, reemplazar la imagen
                if (ImagenAnuncio != null && ImagenAnuncio.Length > 0)
                {
                    try
                    {
                        // Eliminar la imagen anterior si existe
                        if (!string.IsNullOrWhiteSpace(anuncio.UrlImagen))
                        {
                            await _publicidadStorageService.DeleteAnuncioImageAsync(anuncio.UrlImagen);
                        }

                        // Subir la nueva imagen
                        nuevaUrlImagen = await _publicidadStorageService.UploadAnuncioImageAsync(ImagenAnuncio);
                    }
                    catch (ArgumentException ex)
                    {
                        MensajeError = $"Error en la validación de la imagen: {ex.Message}";
                        return RedirectToPage(new { id = EmpresaId });
                    }
                }
                // Si no se subió archivo pero hay una URL nueva diferente, usarla
                else if (!string.IsNullOrWhiteSpace(UrlImagen) && UrlImagen != anuncio.UrlImagen)
                {
                    nuevaUrlImagen = UrlImagen;
                }
                // Si no hay imagen nueva, mantener la actual (permitir editar solo UrlDestino)

                anuncio.UrlImagen = nuevaUrlImagen;
                // Asignar UrlDestino, incluso si es null o vacío
                anuncio.UrlDestino = string.IsNullOrWhiteSpace(UrlDestino) ? null : UrlDestino;
                
                // Marcar explícitamente la entidad como modificada
                _context.Entry(anuncio).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                MensajeExito = "Anuncio actualizado exitosamente";
                return RedirectToPage(new { id = EmpresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al editar el anuncio: {ex.Message}";
                return RedirectToPage(new { id = EmpresaId });
            }
        }

        public async Task<IActionResult> OnPostCambiarEstadoAnuncioAsync(int anuncioId, int empresaId)
        {
            try
            {
                // Usar AsTracking() para forzar el seguimiento de cambios
                var anuncio = await _context.AnunciosPublicidad
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == anuncioId);
                    
                if (anuncio == null)
                {
                    MensajeError = "Anuncio no encontrado";
                    return RedirectToPage(new { id = empresaId });
                }

                anuncio.Activo = !anuncio.Activo;
                
                // Marcar explícitamente la entidad como modificada
                _context.Entry(anuncio).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                var estado = anuncio.Activo ? "activado" : "desactivado";
                MensajeExito = $"Anuncio {estado} exitosamente";
                return RedirectToPage(new { id = empresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cambiar el estado del anuncio: {ex.Message}";
                return RedirectToPage(new { id = empresaId });
            }
        }

        public async Task<IActionResult> OnPostCambiarEstadoCobrosAsync(int empresaId)
        {
            try
            {
                var empresa = await _context.EmpresasPublicidad
                    .AsTracking()
                    .FirstOrDefaultAsync(e => e.Id == empresaId);
                    
                if (empresa == null)
                {
                    MensajeError = "Empresa no encontrada";
                    return RedirectToPage("/Admin/Publicidad");
                }

                // Alternar entre "Al día" y "Pendiente"
                empresa.EstadoCobros = empresa.EstadoCobros == "Al día" ? "Pendiente" : "Al día";
                
                _context.Entry(empresa).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                MensajeExito = $"Estado de cobros actualizado a: {empresa.EstadoCobros}";
                return RedirectToPage(new { id = empresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cambiar el estado de cobros: {ex.Message}";
                return RedirectToPage(new { id = empresaId });
            }
        }

        public async Task<IActionResult> OnPostDeshabilitarTodosAsync(int empresaId)
        {
            try
            {
                // Usar AsTracking() para forzar el seguimiento de cambios
                var anuncios = await _context.AnunciosPublicidad
                    .AsTracking()
                    .Where(a => a.EmpresaPublicidadId == empresaId && a.Activo)
                    .ToListAsync();

                if (!anuncios.Any())
                {
                    MensajeError = "No hay anuncios activos para deshabilitar";
                    return RedirectToPage(new { id = empresaId });
                }

                foreach (var anuncio in anuncios)
                {
                    anuncio.Activo = false;
                }

                // Actualizar la fecha de salida de la empresa
                var empresa = await _context.EmpresasPublicidad.FindAsync(empresaId);
                if (empresa != null)
                {
                    empresa.FechaSalida = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                MensajeExito = $"{anuncios.Count} anuncios deshabilitados exitosamente";
                return RedirectToPage(new { id = empresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al deshabilitar los anuncios: {ex.Message}";
                return RedirectToPage(new { id = empresaId });
            }
        }

        public async Task<IActionResult> OnPostEliminarAnuncioAsync(int anuncioId, int empresaId)
        {
            try
            {
                var anuncio = await _context.AnunciosPublicidad.FindAsync(anuncioId);
                if (anuncio == null)
                {
                    MensajeError = "Anuncio no encontrado";
                    return RedirectToPage(new { id = empresaId });
                }

                // Eliminar la imagen del almacenamiento si existe
                if (!string.IsNullOrWhiteSpace(anuncio.UrlImagen))
                {
                    await _publicidadStorageService.DeleteAnuncioImageAsync(anuncio.UrlImagen);
                }

                _context.AnunciosPublicidad.Remove(anuncio);
                await _context.SaveChangesAsync();

                MensajeExito = "Anuncio eliminado exitosamente";
                return RedirectToPage(new { id = empresaId });
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al eliminar el anuncio: {ex.Message}";
                return RedirectToPage(new { id = empresaId });
            }
        }
    }
}
