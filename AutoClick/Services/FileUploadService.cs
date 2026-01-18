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

public class FileUploadException : Exception
{
    public FileUploadException(string message) : base(message) { }
}

public class FileUploadService : IFileUploadService
{
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB por imagen
    private const long MaxTotalSize = 100 * 1024 * 1024; // 100 MB total
    private const int MaxConcurrency = 4; // Conexiones paralelas según recomendación de Azure
    
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
        var filesList = files.Where(f => f != null && f.Length > 0).ToList();
        
        if (!filesList.Any())
        {
            return new List<string>();
        }

        // Validar tamaño individual de cada archivo
        var oversizedFiles = filesList.Where(f => f.Length > MaxFileSize).ToList();
        if (oversizedFiles.Any())
        {
            var fileNames = string.Join(", ", oversizedFiles.Select(f => f.FileName));
            throw new FileUploadException($"Los siguientes archivos exceden el tamaño máximo de 10 MB: {fileNames}");
        }

        // Validar tamaño total
        var totalSize = filesList.Sum(f => f.Length);
        if (totalSize > MaxTotalSize)
        {
            var totalSizeMB = totalSize / (1024.0 * 1024.0);
            throw new FileUploadException($"El tamaño total de las imágenes ({totalSizeMB:F1} MB) excede el límite de 100 MB.");
        }

        // Subir archivos en paralelo con límite de concurrencia
        var results = new List<string>();
        var semaphore = new SemaphoreSlim(MaxConcurrency);
        var uploadTasks = new List<Task<string>>();

        foreach (var file in filesList)
        {
            uploadTasks.Add(UploadFileWithSemaphoreAsync(file, container, semaphore));
        }

        results = (await Task.WhenAll(uploadTasks)).ToList();
        return results;
    }

    private async Task<string> UploadFileWithSemaphoreAsync(IFormFile file, string container, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            return await UploadFileAsync(file, container);
        }
        finally
        {
            semaphore.Release();
        }
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

        // Configurar opciones de transferencia según recomendaciones de Azure
        var transferOptions = new Azure.Storage.StorageTransferOptions
        {
            InitialTransferSize = 8 * 1024 * 1024, // 8 MB - umbral para subida en chunks
            MaximumTransferSize = 4 * 1024 * 1024, // 4 MB - tamaño de cada chunk (activa high-throughput)
            MaximumConcurrency = 2 // 2 conexiones concurrentes por archivo
        };

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions 
        { 
            HttpHeaders = blobHttpHeaders,
            TransferOptions = transferOptions
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