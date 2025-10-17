using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace AutoClick.Services
{
    public class AzureStorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(string connectionString, ILogger<AzureStorageService> logger)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
            _logger.LogInformation("AzureStorageService initialized");
        }

        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                // Cambiar a PublicAccessType.Blob para permitir acceso público a las imágenes
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                
                var blobClient = containerClient.GetBlobClient(fileName);
                
                // Configurar headers para optimizar cache y rendimiento
                var blobUploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = GetContentType(fileName),
                        CacheControl = "public, max-age=31536000" // Cache por 1 año
                    }
                };
                
                await blobClient.UploadAsync(fileStream, blobUploadOptions, cancellationToken: default);
                
                _logger.LogInformation("File uploaded to Azure Blob Storage: {FileName} in container {ContainerName}", fileName, containerName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to Azure container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".gif" => "image/gif",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }

        public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                
                if (!await blobClient.ExistsAsync())
                {
                    throw new FileNotFoundException($"Blob not found: {fileName}");
                }
                
                var response = await blobClient.DownloadStreamingAsync();
                _logger.LogInformation("File downloaded from Azure Blob Storage: {FileName}", fileName);
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileName} from Azure container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                
                var response = await blobClient.DeleteIfExistsAsync();
                
                if (response.Value)
                {
                    _logger.LogInformation("File deleted from Azure Blob Storage: {FileName}", fileName);
                }
                else
                {
                    _logger.LogWarning("File not found for deletion in Azure Blob Storage: {FileName}", fileName);
                }
                
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileName} from Azure container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public async Task<List<string>> ListFilesAsync(string containerName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                
                if (!await containerClient.ExistsAsync())
                {
                    _logger.LogInformation("Azure container does not exist: {ContainerName}", containerName);
                    return new List<string>();
                }
                
                var files = new List<string>();
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    files.Add(blobItem.Name);
                }
                
                _logger.LogInformation("Listed {Count} files from Azure container {ContainerName}", files.Count, containerName);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files from Azure container {ContainerName}", containerName);
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                
                var exists = await blobClient.ExistsAsync();
                _logger.LogInformation("File exists check in Azure Blob Storage for {FileName}: {Exists}", fileName, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if file exists {FileName} in Azure container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public async Task<long> GetFileSizeAsync(string containerName, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                
                if (!await blobClient.ExistsAsync())
                {
                    throw new FileNotFoundException($"Blob not found: {fileName}");
                }
                
                var properties = await blobClient.GetPropertiesAsync();
                var size = properties.Value.ContentLength;
                
                _logger.LogInformation("File size in Azure Blob Storage for {FileName}: {Size} bytes", fileName, size);
                return size;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size for {FileName} in Azure container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public async Task<string> GenerateSasUrlAsync(string containerName, string fileName, TimeSpan expiry)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                
                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning("Blob not found for SAS URL generation: {FileName}", fileName);
                    return string.Empty;
                }
                
                // Verificar si el blob client puede generar SAS URLs
                if (blobClient.CanGenerateSasUri)
                {
                    var sasBuilder = new BlobSasBuilder
                    {
                        BlobContainerName = containerName,
                        BlobName = fileName,
                        Resource = "b",
                        ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
                    };
                    
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    
                    var sasUri = blobClient.GenerateSasUri(sasBuilder);
                    _logger.LogInformation("Generated SAS URL for {FileName}", fileName);
                    return sasUri.ToString();
                }
                else
                {
                    _logger.LogError("Cannot generate SAS URL for {FileName} - client not configured with account key", fileName);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SAS URL for {FileName} in container {ContainerName}", fileName, containerName);
                return string.Empty;
            }
        }
    }
}