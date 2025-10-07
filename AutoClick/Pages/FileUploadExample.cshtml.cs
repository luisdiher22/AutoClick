using AutoClick.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AutoClick.Pages
{
    public class FileUploadExampleModel : PageModel
    {
        private readonly IStorageService _storageService;
        private readonly ILogger<FileUploadExampleModel> _logger;

        public FileUploadExampleModel(IStorageService storageService, ILogger<FileUploadExampleModel> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile UploadedFile { get; set; } = default!;

        public List<string> UploadedFiles { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // List existing files in the 'uploads' container
                UploadedFiles = await _storageService.ListFilesAsync("uploads");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing files");
                UploadedFiles = new List<string>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                ModelState.AddModelError("UploadedFile", "Please select a file to upload");
                await OnGetAsync(); // Reload the file list
                return Page();
            }

            try
            {
                using var stream = UploadedFile.OpenReadStream();
                var fileName = Path.GetFileName(UploadedFile.FileName);
                
                // Upload to 'uploads' container
                var result = await _storageService.UploadFileAsync("uploads", fileName, stream);
                
                _logger.LogInformation("File uploaded successfully: {FileName}", fileName);
                
                // Redirect to avoid resubmission on page refresh
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", UploadedFile.FileName);
                ModelState.AddModelError("", "An error occurred while uploading the file");
                await OnGetAsync(); // Reload the file list
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDownloadAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            try
            {
                var fileStream = await _storageService.DownloadFileAsync("uploads", fileName);
                var contentType = GetContentType(fileName);
                
                return File(fileStream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
                return StatusCode(500, "An error occurred while downloading the file");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            try
            {
                var deleted = await _storageService.DeleteFileAsync("uploads", fileName);
                
                if (deleted)
                {
                    _logger.LogInformation("File deleted successfully: {FileName}", fileName);
                }
                else
                {
                    _logger.LogWarning("File not found for deletion: {FileName}", fileName);
                }
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
                return StatusCode(500, "An error occurred while deleting the file");
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}