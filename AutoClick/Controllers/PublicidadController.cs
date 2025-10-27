using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using System.ComponentModel.DataAnnotations;

namespace AutoClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicidadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<PublicidadController> _logger;

        public PublicidadController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<PublicidadController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("registrar-click")]
        public async Task<IActionResult> RegistrarClick([FromBody] RegistrarAnuncioRequest request)
        {
            try
            {
                var anuncio = await _context.AnunciosPublicidad
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.AnuncioId);

                if (anuncio == null)
                {
                    return NotFound(new { mensaje = "Anuncio no encontrado" });
                }

                anuncio.NumeroClics++;
                _context.Entry(anuncio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Click registrado correctamente", clics = anuncio.NumeroClics });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar el click", error = ex.Message });
            }
        }

        [HttpPost("registrar-vista")]
        public async Task<IActionResult> RegistrarVista([FromBody] RegistrarAnuncioRequest request)
        {
            try
            {
                var anuncio = await _context.AnunciosPublicidad
                    .AsTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.AnuncioId);

                if (anuncio == null)
                {
                    return NotFound(new { mensaje = "Anuncio no encontrado" });
                }

                anuncio.NumeroVistas++;
                _context.Entry(anuncio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Vista registrada correctamente", vistas = anuncio.NumeroVistas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar la vista", error = ex.Message });
            }
        }

        [HttpPost("solicitar-anuncio")]
        public async Task<IActionResult> SolicitarAnuncio([FromBody] SolicitudAnuncioRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return BadRequest(new { mensaje = "Datos inválidos", errores = errors });
                }

                _logger.LogInformation("Procesando nueva solicitud de anuncio publicitario");

                // Crear y guardar la solicitud en la base de datos
                var solicitud = new SolicitudEmpresa
                {
                    NombreEmpresa = request.NombreEmpresa,
                    RepresentanteLegal = request.RepresentanteLegal,
                    Industria = request.Industria,
                    CorreoElectronico = request.CorreoElectronico,
                    Telefono = request.Telefono,
                    DescripcionEmpresa = request.DescripcionEmpresa,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "Pendiente"
                };

                _context.SolicitudesEmpresa.Add(solicitud);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Solicitud de anuncio guardada en BD con ID: {solicitud.Id}");

                // Obtener correos de todos los administradores
                var correosAdmins = await _context.Usuarios
                    .Where(u => u.EsAdministrador == true)
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!correosAdmins.Any())
                {
                    _logger.LogWarning("No se encontraron administradores en el sistema");
                    correosAdmins.Add("admin@autoclick.cr");
                }

                _logger.LogInformation($"Se encontraron {correosAdmins.Count} administrador(es)");

                // Enviar email a los administradores (indicando que es espacio publicitario)
                bool emailEnviado = await _emailService.EnviarNotificacionSolicitudEmpresaAsync(solicitud, correosAdmins, true);

                if (emailEnviado)
                {
                    _logger.LogInformation("Email de notificación de solicitud de anuncio enviado exitosamente");
                }
                else
                {
                    _logger.LogWarning("No se pudo enviar el email de notificación de solicitud de anuncio");
                }

                return Ok(new
                {
                    mensaje = "Solicitud recibida correctamente",
                    solicitudId = solicitud.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al procesar solicitud de anuncio: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error al procesar la solicitud. Por favor, intente nuevamente." });
            }
        }
    }

    public class RegistrarAnuncioRequest
    {
        public int AnuncioId { get; set; }
    }

    public class SolicitudAnuncioRequest
    {
        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [StringLength(100, ErrorMessage = "El nombre de la empresa no puede exceder 100 caracteres")]
        public string NombreEmpresa { get; set; } = "";

        [Required(ErrorMessage = "El nombre del representante legal es requerido")]
        [StringLength(100, ErrorMessage = "El nombre del representante no puede exceder 100 caracteres")]
        public string RepresentanteLegal { get; set; } = "";

        [Required(ErrorMessage = "La industria es requerida")]
        public string Industria { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [StringLength(150, ErrorMessage = "El correo electrónico no puede exceder 150 caracteres")]
        public string CorreoElectronico { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "La descripción de la empresa es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        [MinLength(10, ErrorMessage = "La descripción debe tener al menos 10 caracteres")]
        public string DescripcionEmpresa { get; set; } = "";
    }
}

