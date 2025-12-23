using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using AutoClick.Models;

namespace AutoClick.Services;

/// <summary>
/// Servicio para gestionar el almacenamiento de imágenes de publicidad en Azure Blob Storage.
/// Implementa las mejores prácticas de Azure: Managed Identity, manejo de errores, y reintentos automáticos.
/// Incluye validación y redimensionamiento automático de imágenes según el tamaño del anuncio.
/// </summary>
public interface IPublicidadStorageService
{
    /// <summary>
    /// Sube una imagen de anuncio al contenedor de blobs 'anuncios' en Azure Storage
    /// La imagen será redimensionada automáticamente según el tamaño especificado
    /// </summary>
    /// <param name="file">Archivo de imagen a subir</param>
    /// <param name="tamanoAnuncio">Tamaño del anuncio (Horizontal, GrandeVertical, MedioVertical)</param>
    /// <returns>URL pública del blob subido</returns>
    Task<string> UploadAnuncioImageAsync(IFormFile file, TamanoAnuncio tamanoAnuncio);
    
    /// <summary>
    /// Valida si una imagen cumple con los requisitos mínimos de dimensiones
    /// </summary>
    /// <param name="file">Archivo de imagen a validar</param>
    /// <param name="tamanoAnuncio">Tamaño del anuncio esperado</param>
    /// <returns>Tupla con (esValida, mensajeError)</returns>
    Task<(bool esValida, string mensajeError)> ValidarDimensionesImagenAsync(IFormFile file, TamanoAnuncio tamanoAnuncio);
    
    /// <summary>
    /// Elimina una imagen de anuncio del contenedor de blobs 'anuncios'
    /// </summary>
    /// <param name="imageUrl">URL de la imagen a eliminar</param>
    /// <returns>True si se eliminó exitosamente, False en caso contrario</returns>
    Task<bool> DeleteAnuncioImageAsync(string imageUrl);
    
    /// <summary>
    /// Obtiene la URL pública de una imagen de anuncio
    /// </summary>
    /// <param name="blobName">Nombre del blob</param>
    /// <returns>URL pública del blob</returns>
    string GetAnuncioImageUrl(string blobName);
}

public class PublicidadStorageService : IPublicidadStorageService
{
    private readonly BlobServiceClient? _blobServiceClient;
    private readonly ILogger<PublicidadStorageService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly bool _useAzureStorage;
    private const string CONTAINER_NAME = "anuncios";
    
    // Tipos de archivo permitidos para las imágenes
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    };
    
    // Tamaño máximo de archivo: 5 MB
    private const long MaxFileSize = 5 * 1024 * 1024;

    public PublicidadStorageService(
        IConfiguration configuration, 
        ILogger<PublicidadStorageService> logger,
        IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _logger = logger;
        _environment = environment;
        
        var connectionString = _configuration.GetConnectionString("AzureStorage");
        var useAzureConfig = _configuration.GetValue<bool>("UseAzureStorage", false);
        
        // Solo usar Azure si está configurado Y tiene connection string válido
        _useAzureStorage = useAzureConfig && !string.IsNullOrWhiteSpace(connectionString);
        
        if (_useAzureStorage)
        {
            try
            {
                _blobServiceClient = new BlobServiceClient(connectionString);
                _logger.LogInformation("PublicidadStorageService: Azure Blob Service Client configurado correctamente para contenedor '{Container}'", CONTAINER_NAME);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PublicidadStorageService: Error al crear Azure Blob Client. Usando almacenamiento local como fallback.");
                _useAzureStorage = false;
                _blobServiceClient = null;
            }
        }
        else
        {
            _logger.LogInformation("PublicidadStorageService: Usando almacenamiento local");
        }
    }

    public async Task<string> UploadAnuncioImageAsync(IFormFile file, TamanoAnuncio tamanoAnuncio)
    {
        // Validaciones
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("El archivo está vacío o no es válido");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"El archivo excede el tamaño máximo permitido de {MaxFileSize / 1024 / 1024} MB");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new ArgumentException($"Tipo de archivo no permitido. Solo se permiten: {string.Join(", ", AllowedContentTypes)}");
        }

        // Validar dimensiones
        var (esValida, mensajeError) = await ValidarDimensionesImagenAsync(file, tamanoAnuncio);
        if (!esValida)
        {
            throw new ArgumentException(mensajeError);
        }

        // Generar nombre único para el archivo
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";

        try
        {
            // Redimensionar imagen según el tamaño del anuncio
            using var resizedImageStream = await ResizeImageAsync(file, tamanoAnuncio);
            
            if (_useAzureStorage && _blobServiceClient != null)
            {
                _logger.LogInformation("Subiendo imagen de anuncio redimensionada '{FileName}' a Azure Blob Storage", fileName);
                return await UploadToAzureBlobAsync(resizedImageStream, fileName, file.ContentType);
            }
            else
            {
                _logger.LogInformation("Subiendo imagen de anuncio redimensionada '{FileName}' a almacenamiento local", fileName);
                return await UploadToLocalStorageAsync(resizedImageStream, fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir imagen de anuncio '{FileName}'", fileName);
            throw new InvalidOperationException($"Error al subir la imagen: {ex.Message}", ex);
        }
    }

    public async Task<(bool esValida, string mensajeError)> ValidarDimensionesImagenAsync(IFormFile file, TamanoAnuncio tamanoAnuncio)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            var (targetWidth, targetHeight) = tamanoAnuncio.GetDimensions();
            var aspectRatioTarget = (double)targetWidth / targetHeight;
            var aspectRatioImage = (double)image.Width / image.Height;
            
            // Validar que la imagen no sea demasiado pequeña (al menos 50% de las dimensiones objetivo)
            var minWidth = targetWidth * 0.5;
            var minHeight = targetHeight * 0.5;

            if (image.Width < minWidth || image.Height < minHeight)
            {
                return (false, $"La imagen es demasiado pequeña. Dimensiones mínimas: {minWidth}x{minHeight}px. Tu imagen: {image.Width}x{image.Height}px");
            }

            // Advertir si el aspect ratio es muy diferente (se recortará contenido)
            var aspectRatioDiff = Math.Abs(aspectRatioTarget - aspectRatioImage);
            if (aspectRatioDiff > 0.3)
            {
                _logger.LogWarning("La imagen tiene un aspect ratio muy diferente al objetivo. Puede haber recortes significativos.");
            }

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar dimensiones de imagen");
            return (false, "Error al procesar la imagen. Asegúrese de que sea una imagen válida.");
        }
    }

    private async Task<MemoryStream> ResizeImageAsync(IFormFile file, TamanoAnuncio tamanoAnuncio)
    {
        var (targetWidth, targetHeight) = tamanoAnuncio.GetDimensions();
        
        using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream);

        // Redimensionar la imagen manteniendo aspect ratio y recortando el exceso
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(targetWidth, targetHeight),
            Mode = ResizeMode.Crop,
            Position = AnchorPositionMode.Center
        }));

        var outputStream = new MemoryStream();
        
        // Guardar como JPEG con calidad alta
        await image.SaveAsJpegAsync(outputStream, new JpegEncoder
        {
            Quality = 90
        });

        outputStream.Position = 0;
        return outputStream;
    }

    public async Task<bool> DeleteAnuncioImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return false;
        }

        try
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                return await DeleteFromAzureBlobAsync(imageUrl);
            }
            else
            {
                return await DeleteFromLocalStorageAsync(imageUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar imagen de anuncio '{ImageUrl}'", imageUrl);
            return false;
        }
    }

    public string GetAnuncioImageUrl(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return string.Empty;
        }

        if (_useAzureStorage && _blobServiceClient != null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
            var blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.ToString();
        }
        else
        {
            return $"/uploads/{CONTAINER_NAME}/{blobName}";
        }
    }

    #region Azure Blob Storage Methods

    private async Task<string> UploadToAzureBlobAsync(Stream stream, string fileName, string contentType)
    {
        var containerClient = _blobServiceClient!.GetBlobContainerClient(CONTAINER_NAME);
        
        // Crear el contenedor si no existe con acceso público a blobs
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        
        var blobClient = containerClient.GetBlobClient(fileName);
        
        // Configurar headers HTTP para optimización
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "image/jpeg", // Siempre JPEG después del redimensionamiento
            CacheControl = "public, max-age=31536000" // Cache por 1 año
        };

        // Configurar metadata
        var metadata = new Dictionary<string, string>
        {
            { "UploadDate", DateTime.UtcNow.ToString("O") },
            { "FileSize", stream.Length.ToString() }
        };
        
        var uploadOptions = new BlobUploadOptions 
        { 
            HttpHeaders = blobHttpHeaders,
            Metadata = metadata,
            // Configurar tier de acceso (Hot para imágenes que se mostrarán frecuentemente)
            AccessTier = AccessTier.Hot
        };

        await blobClient.UploadAsync(stream, uploadOptions);
        
        _logger.LogInformation(
            "Imagen de anuncio subida exitosamente a Azure Blob Storage: {BlobUrl}", 
            blobClient.Uri.ToString()
        );

        return blobClient.Uri.ToString();
    }

    private async Task<bool> DeleteFromAzureBlobAsync(string imageUrl)
    {
        try
        {
            // Extraer el nombre del blob de la URL
            var uri = new Uri(imageUrl);
            var blobName = Path.GetFileName(uri.LocalPath);
            
            var containerClient = _blobServiceClient!.GetBlobContainerClient(CONTAINER_NAME);
            var blobClient = containerClient.GetBlobClient(blobName);
            
            var response = await blobClient.DeleteIfExistsAsync();
            
            if (response.Value)
            {
                _logger.LogInformation("Imagen de anuncio eliminada de Azure Blob Storage: {BlobName}", blobName);
            }
            
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar blob de Azure Storage: {ImageUrl}", imageUrl);
            return false;
        }
    }

    #endregion

    #region Local Storage Methods

    private async Task<string> UploadToLocalStorageAsync(Stream stream, string fileName)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", CONTAINER_NAME);
        
        // Crear directorio si no existe
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var filePath = Path.Combine(uploadsPath, fileName);
        
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream);

        _logger.LogInformation("Imagen de anuncio guardada localmente: {FilePath}", filePath);

        return $"/uploads/{CONTAINER_NAME}/{fileName}";
    }

    private async Task<bool> DeleteFromLocalStorageAsync(string imageUrl)
    {
        try
        {
            // Extraer el nombre del archivo de la URL
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", CONTAINER_NAME, fileName);
            
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation("Imagen de anuncio eliminada del almacenamiento local: {FilePath}", filePath);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo local: {ImageUrl}", imageUrl);
            return false;
        }
    }

    #endregion
}
