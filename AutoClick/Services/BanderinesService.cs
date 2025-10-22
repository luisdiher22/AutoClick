namespace AutoClick.Services
{
    public interface IBanderinesService
    {
        Task<List<string>> GetAllBanderinesUrlsAsync();
        Task<string> GetBanderinUrlAsync(string banderineName);
        Task<string> GetLogoUrlAsync(string logoName);
        Task<bool> MigrateBanderinesToBlobAsync();
        Task<bool> UploadBanderinAsync(string fileName, Stream fileStream);
    }

    public class BanderinesService : IBanderinesService
    {
        private readonly IStorageService _storageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BanderinesService> _logger;
        private readonly string _containerName = "banderines";
        private readonly string _localPath;
        private readonly string _baseUrl;

        public BanderinesService(
            IStorageService storageService, 
            IConfiguration configuration, 
            ILogger<BanderinesService> logger,
            IWebHostEnvironment environment)
        {
            _storageService = storageService;
            _configuration = configuration;
            _logger = logger;
            _localPath = Path.Combine(environment.WebRootPath, "images", "Banderines");
            
            // URL base para acceso directo a blob storage
            var storageAccount = _configuration["AzureStorage:AccountName"];
            _baseUrl = $"https://{storageAccount}.blob.core.windows.net/{_containerName}";
        }

        public async Task<List<string>> GetAllBanderinesUrlsAsync()
        {
            try
            {
                var blobs = await _storageService.ListFilesAsync(_containerName);
                var urls = new List<string>();
                
                foreach (var blob in blobs)
                {
                    var sasUrl = await _storageService.GenerateSasUrlAsync(_containerName, blob, TimeSpan.FromHours(1));
                    if (!string.IsNullOrEmpty(sasUrl))
                    {
                        urls.Add(sasUrl);
                    }
                }
                
                return urls;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banderines URLs from blob storage");
                
                // Fallback a archivos locales si falla el blob storage
                return GetLocalBanderinesUrls();
            }
        }

        public async Task<string> GetBanderinUrlAsync(string banderineName)
        {
            try
            {
                // Verificar si existe en blob storage
                var exists = await _storageService.FileExistsAsync(_containerName, banderineName);
                if (exists)
                {
                    var sasUrl = await _storageService.GenerateSasUrlAsync(_containerName, banderineName, TimeSpan.FromHours(1));
                    if (!string.IsNullOrEmpty(sasUrl))
                    {
                        return sasUrl;
                    }
                }
                
                // Fallback a archivo local
                var localFile = Path.Combine(_localPath, banderineName);
                if (File.Exists(localFile))
                {
                    return $"/images/Banderines/{banderineName}";
                }
                
                _logger.LogWarning("Banderin not found: {BanderinName}", banderineName);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banderin URL for {BanderinName}", banderineName);
                return $"/images/Banderines/{banderineName}"; // Fallback
            }
        }

        public async Task<string> GetLogoUrlAsync(string logoName)
        {
            try
            {
                string logosContainer = "logos";
                
                // Verificar si existe en blob storage (contenedor de logos)
                var exists = await _storageService.FileExistsAsync(logosContainer, logoName);
                if (exists)
                {
                    var sasUrl = await _storageService.GenerateSasUrlAsync(logosContainer, logoName, TimeSpan.FromHours(1));
                    if (!string.IsNullOrEmpty(sasUrl))
                    {
                        return sasUrl;
                    }
                }
                
                _logger.LogWarning("Logo not found in blob storage: {LogoName}", logoName);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logo URL for {LogoName}", logoName);
                return string.Empty;
            }
        }

        public async Task<bool> MigrateBanderinesToBlobAsync()
        {
            try
            {
                if (!Directory.Exists(_localPath))
                {
                    _logger.LogWarning("Local banderines directory not found: {Path}", _localPath);
                    return false;
                }

                var files = Directory.GetFiles(_localPath, "*.gif");
                var uploadTasks = new List<Task<bool>>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    uploadTasks.Add(UploadFileToBlob(file, fileName));
                }

                var results = await Task.WhenAll(uploadTasks);
                var successCount = results.Count(r => r);
                
                _logger.LogInformation("Migration completed: {SuccessCount}/{TotalCount} files uploaded", 
                    successCount, results.Length);
                
                return successCount == results.Length;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during banderines migration");
                return false;
            }
        }

        public async Task<bool> UploadBanderinAsync(string fileName, Stream fileStream)
        {
            try
            {
                await _storageService.UploadFileAsync(_containerName, fileName, fileStream);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading banderin {FileName}", fileName);
                return false;
            }
        }

        private async Task<bool> UploadFileToBlob(string filePath, string fileName)
        {
            try
            {
                using var fileStream = File.OpenRead(filePath);
                await _storageService.UploadFileAsync(_containerName, fileName, fileStream);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName} to blob", fileName);
                return false;
            }
        }

        private List<string> GetLocalBanderinesUrls()
        {
            try
            {
                if (!Directory.Exists(_localPath))
                    return new List<string>();

                var files = Directory.GetFiles(_localPath, "*.gif");
                return files.Select(f => $"/images/Banderines/{Path.GetFileName(f)}").ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting local banderines URLs");
                return new List<string>();
            }
        }
    }
}