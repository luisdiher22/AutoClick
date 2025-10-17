using Microsoft.AspNetCore.Mvc;
using AutoClick.Services;

namespace AutoClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BanderinesController : ControllerBase
    {
        private readonly IBanderinesService _banderinesService;
        private readonly ILogger<BanderinesController> _logger;

        public BanderinesController(IBanderinesService banderinesService, ILogger<BanderinesController> logger)
        {
            _banderinesService = banderinesService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene las URLs de todos los banderines disponibles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetBanderines()
        {
            try
            {
                var urls = await _banderinesService.GetAllBanderinesUrlsAsync();
                return Ok(urls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banderines");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la URL de un banderin específico
        /// </summary>
        [HttpGet("{banderinName}")]
        public async Task<ActionResult<string>> GetBanderin(string banderinName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(banderinName))
                    return BadRequest("Nombre del banderin requerido");

                var url = await _banderinesService.GetBanderinUrlAsync(banderinName);
                
                if (string.IsNullOrEmpty(url))
                    return NotFound($"Banderin '{banderinName}' no encontrado");

                return Ok(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting banderin {BanderinName}", banderinName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Migra todos los banderines locales a Azure Blob Storage
        /// </summary>
        [HttpPost("migrate")]
        public async Task<ActionResult> MigrateBanderines()
        {
            try
            {
                _logger.LogInformation("Starting banderines migration to Azure Blob Storage");
                
                var success = await _banderinesService.MigrateBanderinesToBlobAsync();
                
                if (success)
                {
                    _logger.LogInformation("Banderines migration completed successfully");
                    return Ok(new { message = "Migración completada exitosamente", success = true });
                }
                else
                {
                    _logger.LogWarning("Banderines migration completed with errors");
                    return Ok(new { message = "Migración completada con algunos errores", success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during banderines migration");
                return StatusCode(500, new { message = "Error durante la migración", error = ex.Message });
            }
        }

        /// <summary>
        /// Sube un nuevo banderin a Azure Blob Storage
        /// </summary>
        [HttpPost("upload")]
        public async Task<ActionResult> UploadBanderin(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Archivo requerido");

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".gif", ".png", ".jpg", ".jpeg" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Solo se permiten archivos de imagen (gif, png, jpg, jpeg)");

                // Validar tamaño (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("El archivo no puede ser mayor a 5MB");

                using var stream = file.OpenReadStream();
                var success = await _banderinesService.UploadBanderinAsync(file.FileName, stream);

                if (success)
                {
                    var url = await _banderinesService.GetBanderinUrlAsync(file.FileName);
                    return Ok(new { message = "Archivo subido exitosamente", fileName = file.FileName, url });
                }
                else
                {
                    return StatusCode(500, "Error subiendo el archivo");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading banderin file");
                return StatusCode(500, new { message = "Error subiendo archivo", error = ex.Message });
            }
        }
    }
}