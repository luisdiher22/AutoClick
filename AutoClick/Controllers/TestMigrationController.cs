using Microsoft.AspNetCore.Mvc;
using AutoClick.Services;

namespace AutoClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestMigrationController : ControllerBase
    {
        private readonly IBanderinesService _banderinesService;
        private readonly ILogger<TestMigrationController> _logger;

        public TestMigrationController(IBanderinesService banderinesService, ILogger<TestMigrationController> logger)
        {
            _banderinesService = banderinesService;
            _logger = logger;
        }

        /// <summary>
        /// Ejecutar migración usando la configuración de appsettings.json
        /// </summary>
        [HttpPost("migrate-now")]
        public async Task<ActionResult> MigrateNow()
        {
            try
            {
                _logger.LogInformation("Starting migration using configured Azure Storage");
                
                var success = await _banderinesService.MigrateBanderinesToBlobAsync();
                
                if (success)
                {
                    var urls = await _banderinesService.GetAllBanderinesUrlsAsync();
                    return Ok(new 
                    { 
                        success = true,
                        message = "Migración completada exitosamente",
                        totalFiles = urls.Count,
                        sampleUrls = urls.Take(3).ToList(),
                        baseUrl = "https://autoclickstorage.blob.core.windows.net/banderines/"
                    });
                }
                else
                {
                    return Ok(new 
                    { 
                        success = false,
                        message = "Migración completada con algunos errores"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during migration");
                return StatusCode(500, new { success = false, message = "Error durante la migración", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener estado de la migración
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetStatus()
        {
            try
            {
                var urls = await _banderinesService.GetAllBanderinesUrlsAsync();
                var isUsingAzure = urls.Any(u => u.Contains("blob.core.windows.net"));
                
                return Ok(new
                {
                    isUsingAzure,
                    totalFiles = urls.Count,
                    azureFiles = urls.Count(u => u.Contains("blob.core.windows.net")),
                    localFiles = urls.Count(u => !u.Contains("blob.core.windows.net")),
                    sampleUrls = urls.Take(3).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting migration status");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}