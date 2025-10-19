using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AutoClick.Services;

namespace AutoClick.Pages.Admin
{
    public class ImportarVentasExternasModel : PageModel
    {
        private readonly IVentasExternasService _ventasExternasService;
        private readonly ILogger<ImportarVentasExternasModel> _logger;

        public ImportarVentasExternasModel(
            IVentasExternasService ventasExternasService,
            ILogger<ImportarVentasExternasModel> logger)
        {
            _ventasExternasService = ventasExternasService;
            _logger = logger;
        }

        [BindProperty]
        public IFormFile? ArchivoExcel { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public int TotalRegistros { get; set; }
        public DateTime? UltimaImportacion { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar autenticación del administrador
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Auth");
            }

            // Aquí puedes agregar validación adicional para verificar que es administrador
            // Por ejemplo, verificar rol o email específico

            TotalRegistros = await _ventasExternasService.ObtenerTotalRegistros();
            UltimaImportacion = await _ventasExternasService.ObtenerFechaUltimaImportacion();
            
            return Page();
        }

        public async Task<IActionResult> OnPostImportarAsync()
        {
            if (ArchivoExcel == null || ArchivoExcel.Length == 0)
            {
                ErrorMessage = "Por favor seleccione un archivo Excel para importar.";
                return RedirectToPage();
            }

            // Validar tamaño del archivo (máximo 10MB)
            if (ArchivoExcel.Length > 10 * 1024 * 1024)
            {
                ErrorMessage = "El archivo es demasiado grande. El tamaño máximo permitido es 10MB.";
                return RedirectToPage();
            }

            try
            {
                using (var stream = ArchivoExcel.OpenReadStream())
                {
                    var (success, message, recordsImported) = await _ventasExternasService.ImportarExcelAsync(
                        stream, 
                        ArchivoExcel.FileName
                    );

                    if (success)
                    {
                        SuccessMessage = message;
                        _logger.LogInformation($"Importación exitosa: {recordsImported} registros importados por el usuario {User.Identity?.Name}");
                    }
                    else
                    {
                        ErrorMessage = message;
                        _logger.LogWarning($"Error en importación: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error inesperado al procesar el archivo: {ex.Message}";
                _logger.LogError(ex, "Error inesperado durante la importación");
            }

            return RedirectToPage();
        }
    }
}
