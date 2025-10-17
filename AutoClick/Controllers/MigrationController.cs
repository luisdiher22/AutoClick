using Microsoft.AspNetCore.Mvc;
using AutoClick.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AutoClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MigrationController> _logger;

        public MigrationController(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ILogger<MigrationController> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para migrar banderines usando connection string proporcionado
        /// </summary>
        [HttpPost("banderines")]
        public async Task<ActionResult> MigrateBanderines([FromBody] MigrationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ConnectionString))
                    return BadRequest("Connection string es requerido");

                var localPath = Path.Combine(_environment.WebRootPath, "images", "Banderines");
                
                if (!Directory.Exists(localPath))
                    return BadRequest($"Directorio no encontrado: {localPath}");

                var gifFiles = Directory.GetFiles(localPath, "*.gif");
                
                if (gifFiles.Length == 0)
                    return Ok(new { message = "No se encontraron archivos GIF para migrar", uploaded = 0 });

                // Configurar Azure Blob Storage
                var blobServiceClient = new BlobServiceClient(request.ConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("banderines");
                
                // Crear container si no existe
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var results = new List<MigrationResult>();
                var uploadedCount = 0;

                foreach (var filePath in gifFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    try
                    {
                        var blobClient = containerClient.GetBlobClient(fileName);
                        
                        var blobUploadOptions = new BlobUploadOptions
                        {
                            HttpHeaders = new BlobHttpHeaders
                            {
                                ContentType = "image/gif",
                                CacheControl = "public, max-age=31536000"
                            }
                        };

                        using var fileStream = System.IO.File.OpenRead(filePath);
                        await blobClient.UploadAsync(fileStream, blobUploadOptions);
                        
                        uploadedCount++;
                        results.Add(new MigrationResult 
                        { 
                            FileName = fileName, 
                            Success = true, 
                            Url = blobClient.Uri.ToString()
                        });

                        _logger.LogInformation("Uploaded {FileName} to Azure Blob Storage", fileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading {FileName}", fileName);
                        results.Add(new MigrationResult 
                        { 
                            FileName = fileName, 
                            Success = false, 
                            Error = ex.Message 
                        });
                    }
                }

                return Ok(new
                {
                    message = $"Migración completada: {uploadedCount}/{gifFiles.Length} archivos subidos",
                    totalFiles = gifFiles.Length,
                    uploadedCount,
                    baseUrl = $"https://{request.StorageAccountName}.blob.core.windows.net/banderines/",
                    results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during migration");
                return StatusCode(500, new { message = "Error durante la migración", error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para probar la conexión con Azure Storage
        /// </summary>
        [HttpPost("test-connection")]
        public async Task<ActionResult> TestConnection([FromBody] TestConnectionRequest request)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(request.ConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("banderines");
                
                // Intentar crear container (sin error si ya existe)
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                
                // Verificar que podemos listar
                await foreach (var blob in containerClient.GetBlobsAsync())
                {
                    break; // Solo necesitamos probar que funciona
                }

                return Ok(new { 
                    success = true, 
                    message = "Conexión exitosa",
                    containerUrl = containerClient.Uri.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed");
                return Ok(new { 
                    success = false, 
                    message = "Error de conexión", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Obtener información sobre archivos locales
        /// </summary>
        [HttpGet("local-info")]
        public ActionResult GetLocalInfo()
        {
            try
            {
                var localPath = Path.Combine(_environment.WebRootPath, "images", "Banderines");
                
                if (!Directory.Exists(localPath))
                    return Ok(new { exists = false, path = localPath });

                var gifFiles = Directory.GetFiles(localPath, "*.gif");
                var totalSize = gifFiles.Sum(f => new FileInfo(f).Length);

                return Ok(new
                {
                    exists = true,
                    path = localPath,
                    fileCount = gifFiles.Length,
                    totalSizeBytes = totalSize,
                    totalSizeMB = Math.Round(totalSize / (1024.0 * 1024.0), 2),
                    files = gifFiles.Select(f => Path.GetFileName(f)).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting local info");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class MigrationRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string StorageAccountName { get; set; } = "autoclickbanderines";
    }

    public class TestConnectionRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class MigrationResult
    {
        public string FileName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? Error { get; set; }
    }
}