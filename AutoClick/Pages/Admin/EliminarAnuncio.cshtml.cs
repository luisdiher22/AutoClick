using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;

namespace AutoClick.Pages.Admin
{
    public class EliminarAnuncioModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EliminarAnuncioModel> _logger;

        public EliminarAnuncioModel(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<EliminarAnuncioModel> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }
        public Auto? AnuncioAEliminar { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar que el usuario sea administrador
            var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
            if (!isAdmin)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostBuscarAnuncioAsync(int autoId)
        {
            // Verificar que el usuario sea administrador
            var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
            if (!isAdmin)
            {
                return Forbid();
            }

            if (autoId <= 0)
            {
                MensajeError = "Por favor, ingrese un ID de anuncio válido.";
                return Page();
            }

            // Buscar el anuncio
            AnuncioAEliminar = await _context.Autos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == autoId);

            if (AnuncioAEliminar == null)
            {
                MensajeError = $"No se encontró ningún anuncio con el ID {autoId}.";
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmarEliminacionAsync(int autoIdConfirmar)
        {
            // Verificar que el usuario sea administrador
            var isAdmin = User.FindFirst("IsAdmin")?.Value == "true";
            if (!isAdmin)
            {
                return Forbid();
            }

            if (autoIdConfirmar <= 0)
            {
                MensajeError = "ID de anuncio inválido.";
                return Page();
            }

            try
            {
                // Buscar el anuncio
                var auto = await _context.Autos
                    .FirstOrDefaultAsync(a => a.Id == autoIdConfirmar);

                if (auto == null)
                {
                    MensajeError = $"No se encontró el anuncio con ID {autoIdConfirmar}.";
                    return Page();
                }

                var marca = auto.Marca;
                var modelo = auto.Modelo;
                var anno = auto.Ano;

                // Eliminar imágenes del Azure Blob Storage
                var imagenesUrls = auto.ImagenesUrlsList;
                if (imagenesUrls != null && imagenesUrls.Any())
                {
                    await EliminarImagenesBlobAsync(imagenesUrls);
                }

                // Eliminar videos si existen
                var videosUrls = auto.VideosUrlsList;
                if (videosUrls != null && videosUrls.Any())
                {
                    foreach (var videoUrl in videosUrls)
                    {
                        await EliminarVideoBlobAsync(videoUrl);
                    }
                }

                // Nota: Los mensajes no están vinculados directamente a autos en este modelo
                // Si se necesita eliminar mensajes relacionados, se debe implementar una relación

                // Eliminar pagos de Onvo asociados
                var pagosOnvo = await _context.PagosOnvo
                    .Where(p => p.AutoId == autoIdConfirmar)
                    .ToListAsync();

                if (pagosOnvo.Any())
                {
                    _context.PagosOnvo.RemoveRange(pagosOnvo);
                    _logger.LogInformation("Eliminando {Count} pagos Onvo asociados al anuncio {AutoId}", pagosOnvo.Count, autoIdConfirmar);
                }

                // Eliminar favoritos relacionados
                var favoritos = await _context.Favoritos
                    .Where(f => f.AutoId == autoIdConfirmar)
                    .ToListAsync();

                if (favoritos.Any())
                {
                    _context.Favoritos.RemoveRange(favoritos);
                    _logger.LogInformation("Eliminando {Count} favoritos asociados al anuncio {AutoId}", favoritos.Count, autoIdConfirmar);
                }

                // Eliminar el anuncio
                _context.Autos.Remove(auto);
                await _context.SaveChangesAsync();

                _logger.LogWarning("ADMIN: Anuncio eliminado - ID: {AutoId}, Vehículo: {Marca} {Modelo} {Anno}", 
                    autoIdConfirmar, marca, modelo, anno);

                MensajeExito = $"El anuncio #{autoIdConfirmar} ({marca} {modelo} {anno}) ha sido eliminado exitosamente junto con todos sus datos asociados.";
                AnuncioAEliminar = null;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar anuncio {AutoId}: {Message}", autoIdConfirmar, ex.Message);
                MensajeError = $"Error al eliminar el anuncio: {ex.Message}";
                return Page();
            }
        }

        private async Task EliminarImagenesBlobAsync(List<string> imagenesUrls)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning("No se encontró la cadena de conexión de Azure Storage");
                    return;
                }

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("autos");

                foreach (var imageUrl in imagenesUrls)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        try
                        {
                            // Extraer el nombre del blob de la URL
                            var uri = new Uri(imageUrl);
                            var blobName = uri.Segments.Length > 2 
                                ? string.Join("", uri.Segments.Skip(2)) 
                                : Path.GetFileName(uri.LocalPath);

                            var blobClient = containerClient.GetBlobClient(blobName);
                            await blobClient.DeleteIfExistsAsync();
                            _logger.LogInformation("Imagen eliminada del blob: {BlobName}", blobName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "No se pudo eliminar la imagen del blob: {Url}", imageUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar imágenes del blob storage");
            }
        }

        private async Task EliminarVideoBlobAsync(string videoUrl)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("AzureStorage");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return;
                }

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("autos");

                var uri = new Uri(videoUrl);
                var blobName = uri.Segments.Length > 2 
                    ? string.Join("", uri.Segments.Skip(2)) 
                    : Path.GetFileName(uri.LocalPath);

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
                _logger.LogInformation("Video eliminado del blob: {BlobName}", blobName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo eliminar el video del blob: {Url}", videoUrl);
            }
        }
    }
}
