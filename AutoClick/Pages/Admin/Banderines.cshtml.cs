using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;

namespace AutoClick.Pages.Admin
{
    public class AdminBanderinesModel : PageModel
    {
        private readonly IBanderinesService _banderinesService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AdminBanderinesModel(
            IBanderinesService banderinesService,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _banderinesService = banderinesService;
            _configuration = configuration;
            _environment = environment;
        }

        public int LocalFilesCount { get; set; }
        public int AzureFilesCount { get; set; }
        public bool IsUsingAzure { get; set; }

        public async Task OnGetAsync()
        {
            // Contar archivos locales
            var localPath = Path.Combine(_environment.WebRootPath, "images", "Banderines");
            LocalFilesCount = Directory.Exists(localPath) 
                ? Directory.GetFiles(localPath, "*.gif").Length 
                : 0;

            // Contar archivos en Azure
            try
            {
                var azureUrls = await _banderinesService.GetAllBanderinesUrlsAsync();
                AzureFilesCount = azureUrls.Count;
            }
            catch
            {
                AzureFilesCount = 0;
            }

            // Verificar si está usando Azure
            IsUsingAzure = _configuration.GetValue<bool>("UseAzureStorage");
        }
    }
}