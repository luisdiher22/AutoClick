namespace AutoClick.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream);
        Task<Stream> DownloadFileAsync(string containerName, string fileName);
        Task<bool> DeleteFileAsync(string containerName, string fileName);
        Task<List<string>> ListFilesAsync(string containerName);
        Task<bool> FileExistsAsync(string containerName, string fileName);
        Task<long> GetFileSizeAsync(string containerName, string fileName);
    }
}