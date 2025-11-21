using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AutoClick.Services;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(IFormFile file, string container = "autos");
    Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string container = "autos");
    Task<bool> DeleteFileAsync(string fileName, string container = "autos");
    string GetFileUrl(string fileName, string container = "autos");
}

public class FileUploadService : IFileUploadService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly BlobServiceClient? _blobServiceClient;
    private readonly bool _useAzureStorage;

    public FileUploadService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
        
        var connectionString = _configuration.GetConnectionString("AzureStorage");
        if (!string.IsNullOrEmpty(connectionString))
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _useAzureStorage = true;
        }
        else
        {
            _useAzureStorage = false;
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string container = "autos")
    {
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        
        if (_useAzureStorage && _blobServiceClient != null)
        {
            return await UploadToAzureAsync(file, fileName, container);
        }
        else
        {
            return await UploadToLocalAsync(file, fileName, container);
        }
    }

    public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string container = "autos")
    {
        var results = new List<string>();
        
        foreach (var file in files)
        {
            if (file != null && file.Length > 0)
            {
                var result = await UploadFileAsync(file, container);
                results.Add(result);
            }
        }

        return results;
    }

    public async Task<bool> DeleteFileAsync(string fileName, string container = "autos")
    {
        try
        {
            if (_useAzureStorage && _blobServiceClient != null)
            {
                return await DeleteFromAzureAsync(fileName, container);
            }
            else
            {
                return await DeleteFromLocalAsync(fileName, container);
            }
        }
        catch
        {
            return false;
        }
    }

    public string GetFileUrl(string fileName, string container = "autos")
    {
        if (_useAzureStorage && _blobServiceClient != null)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);
            var blobClient = containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }
        else
        {
            return $"/uploads/{container}/{fileName}";
        }
    }

    private async Task<string> UploadToAzureAsync(IFormFile file, string fileName, string container)
    {
        var containerClient = _blobServiceClient!.GetBlobContainerClient(container);
        
        // Crear el contenedor si no existe
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        
        var blobClient = containerClient.GetBlobClient(fileName);
        
        // Configurar headers para optimización web
        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = file.ContentType,
            CacheControl = "public, max-age=31536000" // Cache por 1 año
        };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions 
        { 
            HttpHeaders = blobHttpHeaders 
        });

        return blobClient.Uri.ToString();
    }

    private async Task<string> UploadToLocalAsync(IFormFile file, string fileName, string container)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", container);
        
        // Crear directorio si no existe
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var filePath = Path.Combine(uploadsPath, fileName);
        
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{container}/{fileName}";
    }

    private async Task<bool> DeleteFromAzureAsync(string fileName, string container)
    {
        var containerClient = _blobServiceClient!.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(fileName);
        
        var response = await blobClient.DeleteIfExistsAsync();
        return response.Value;
    }

    private async Task<bool> DeleteFromLocalAsync(string fileName, string container)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", container, fileName);
        
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
            return true;
        }

        return false;
    }
}