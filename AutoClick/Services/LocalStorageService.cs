namespace AutoClick.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(ILogger<LocalStorageService> logger, string basePath = "LocalStorage")
        {
            _logger = logger;
            _basePath = basePath;
            Directory.CreateDirectory(_basePath);
            _logger.LogInformation("LocalStorageService initialized with base path: {BasePath}", _basePath);
        }

        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            try
            {
                var containerPath = Path.Combine(_basePath, containerName);
                Directory.CreateDirectory(containerPath);
                
                var filePath = Path.Combine(containerPath, fileName);
                
                using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
                await fileStream.CopyToAsync(fileStreamOutput);
                
                _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public Task<Stream> DownloadFileAsync(string containerName, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, containerName, fileName);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    throw new FileNotFoundException($"File not found: {fileName}");
                }
                
                _logger.LogInformation("File downloaded successfully: {FilePath}", filePath);
                return Task.FromResult<Stream>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileName} from container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, containerName, fileName);
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return Task.FromResult(true);
                }
                
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileName} from container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public Task<List<string>> ListFilesAsync(string containerName)
        {
            try
            {
                var containerPath = Path.Combine(_basePath, containerName);
                
                if (!Directory.Exists(containerPath))
                {
                    _logger.LogInformation("Container directory does not exist: {ContainerPath}", containerPath);
                    return Task.FromResult(new List<string>());
                }
                
                var files = Directory.GetFiles(containerPath)
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .ToList();
                
                _logger.LogInformation("Listed {Count} files from container {ContainerName}", files.Count, containerName);
                return Task.FromResult(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files from container {ContainerName}", containerName);
                throw;
            }
        }

        public Task<bool> FileExistsAsync(string containerName, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, containerName, fileName);
                var exists = File.Exists(filePath);
                _logger.LogInformation("File exists check for {FilePath}: {Exists}", filePath, exists);
                return Task.FromResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if file exists {FileName} in container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public Task<long> GetFileSizeAsync(string containerName, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_basePath, containerName, fileName);
                
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {fileName}");
                }
                
                var fileInfo = new FileInfo(filePath);
                var size = fileInfo.Length;
                _logger.LogInformation("File size for {FilePath}: {Size} bytes", filePath, size);
                return Task.FromResult(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size for {FileName} in container {ContainerName}", fileName, containerName);
                throw;
            }
        }

        public Task<string> GenerateSasUrlAsync(string containerName, string fileName, TimeSpan expiry)
        {
            try
            {
                var filePath = Path.Combine(_basePath, containerName, fileName);
                
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found for URL generation: {FilePath}", filePath);
                    return Task.FromResult(string.Empty);
                }
                
                // Para storage local, devolvemos una URL relativa
                var url = $"/LocalStorage/{containerName}/{fileName}";
                _logger.LogInformation("Generated local URL for {FileName}: {Url}", fileName, url);
                return Task.FromResult(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating URL for {FileName} in container {ContainerName}", fileName, containerName);
                return Task.FromResult(string.Empty);
            }
        }
    }
}