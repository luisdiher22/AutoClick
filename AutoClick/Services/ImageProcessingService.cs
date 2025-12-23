using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AutoClick.Services
{
    public interface IImageProcessingService
    {
        Task<string> ProcessAndSaveAdvertisementImageAsync(Stream imageStream, string fileName, int targetWidth, int targetHeight);
    }

    public class ImageProcessingService : IImageProcessingService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageProcessingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly bool _useAzureStorage;
        private const string CONTAINER_NAME = "anuncios";

        public ImageProcessingService(
            IWebHostEnvironment environment, 
            ILogger<ImageProcessingService> logger,
            IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;

            var connectionString = _configuration.GetConnectionString("AzureStorage");
            var useAzureConfig = _configuration.GetValue<bool>("UseAzureStorage", false);
            
            // Solo usar Azure si está configurado Y tiene connection string válido
            _useAzureStorage = useAzureConfig && !string.IsNullOrWhiteSpace(connectionString);
            
            if (_useAzureStorage)
            {
                try
                {
                    _blobServiceClient = new BlobServiceClient(connectionString);
                    _logger.LogInformation("ImageProcessingService: Azure Blob Service Client configurado correctamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ImageProcessingService: Error al crear Azure Blob Client. Usando almacenamiento local como fallback.");
                    _useAzureStorage = false;
                    _blobServiceClient = null;
                }
            }
            else
            {
                _logger.LogInformation("ImageProcessingService: Usando almacenamiento local");
            }
        }

        public async Task<string> ProcessAndSaveAdvertisementImageAsync(Stream imageStream, string fileName, int targetWidth, int targetHeight)
        {
            try
            {
                using var image = await Image.LoadAsync(imageStream);

                // Validar dimensiones mínimas (al menos 50% de las dimensiones objetivo)
                var minWidth = targetWidth * 0.5;
                var minHeight = targetHeight * 0.5;

                if (image.Width < minWidth || image.Height < minHeight)
                {
                    throw new ArgumentException($"La imagen es demasiado pequeña. Dimensiones mínimas: {minWidth}x{minHeight}px. Tu imagen: {image.Width}x{image.Height}px");
                }

                // Advertir si el aspect ratio es muy diferente (se recortará contenido)
                var aspectRatioTarget = (double)targetWidth / targetHeight;
                var aspectRatioImage = (double)image.Width / image.Height;
                var aspectRatioDiff = Math.Abs(aspectRatioTarget - aspectRatioImage);
                
                if (aspectRatioDiff > 0.3)
                {
                    _logger.LogWarning("La imagen tiene un aspect ratio muy diferente al objetivo. Puede haber recortes significativos. Target: {TargetRatio}, Image: {ImageRatio}", aspectRatioTarget, aspectRatioImage);
                }

                // Redimensionar la imagen manteniendo aspect ratio y recortando el exceso (Crop mode)
                // Esto garantiza que la imagen llene completamente el espacio sin bordes vacíos
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(targetWidth, targetHeight),
                    Mode = ResizeMode.Crop, // Cambiado de Max a Crop para evitar letterboxing
                    Position = AnchorPositionMode.Center // Recortar desde el centro
                }));

                // Guardar la imagen procesada
                return await SaveImageAsync(image, fileName);
            }
            catch (ArgumentException)
            {
                // Re-lanzar excepciones de validación
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar imagen publicitaria");
                throw new InvalidOperationException("Error al procesar la imagen. Asegúrese de que sea una imagen válida.", ex);
            }
        }

        private async Task<string> SaveImageAsync(Image image, string fileName)
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                return await SaveToAzureBlobAsync(image, fileName);
            }
            else
            {
                return await SaveToLocalStorageAsync(image, fileName);
            }
        }

        private async Task<string> SaveToAzureBlobAsync(Image image, string fileName)
        {
            try
            {
                // Obtener el contenedor
                var containerClient = _blobServiceClient!.GetBlobContainerClient(CONTAINER_NAME);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Generar nombre único
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}.jpg";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                // Convertir la imagen a stream
                using var memoryStream = new MemoryStream();
                await image.SaveAsync(memoryStream, new JpegEncoder { Quality = 90 });
                memoryStream.Position = 0;

                // Configurar headers
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/jpeg",
                    CacheControl = "public, max-age=31536000" // Cache por 1 año
                };

                // Subir al blob
                await blobClient.UploadAsync(memoryStream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                _logger.LogInformation("Imagen procesada guardada en Azure Blob: {BlobName}", uniqueFileName);
                
                // Retornar la URL pública del blob
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar imagen procesada en Azure Blob. Intentando almacenamiento local como fallback.");
                return await SaveToLocalStorageAsync(image, fileName);
            }
        }

        private async Task<string> SaveToLocalStorageAsync(Image image, string fileName)
        {
            // Crear directorio si no existe
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "publicidad");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generar nombre único
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Guardar la imagen
            await image.SaveAsync(filePath, new JpegEncoder { Quality = 90 });

            _logger.LogInformation("Imagen procesada guardada localmente: {FileName}", uniqueFileName);

            // Retornar la URL relativa
            return $"/uploads/publicidad/{uniqueFileName}";
        }
    }
}
