using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
                
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(fileStream, overwrite: true);
                
                _logger.LogInformation("File uploaded to Azure Blob Storage: {FileName} in container {ContainerName}", fileName, containerName);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to Azure container {ContainerName}", fileName, containerName);
                throw;
            }
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
    }
}