using AutoClick.Data;
using AutoClick.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoClick.Controllers.Api
{
    /// <summary>
    /// API Controller para gestionar pagos con ONVO Pay
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly IOnvoPayService _onvoPayService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PagosController> _logger;

        public PagosController(
            IOnvoPayService onvoPayService,
            ApplicationDbContext context,
            ILogger<PagosController> logger)
        {
            _onvoPayService = onvoPayService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva sesión de pago (Payment Intent) para un anuncio
        /// </summary>
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentRequest request)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    return BadRequest(new { error = "El monto debe ser mayor a cero" });
                }

                // Obtener usuario autenticado (opcional)
                int? usuarioId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int parsedUserId))
                    {
                        usuarioId = parsedUserId;
                    }
                }

                var metadata = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(request.OrderReference))
                {
                    metadata["orderReference"] = request.OrderReference;
                }

                // Obtener descripción detallada si es para un auto
                string description = request.Description ?? "Pago AutoClick";
                if (request.AutoId.HasValue)
                {
                    var auto = await _context.Autos.FindAsync(request.AutoId.Value);
                    if (auto != null)
                    {
                        description = $"Publicación de anuncio: {auto.Marca} {auto.Modelo} {auto.Ano}";
                    }
                }
                
                var paymentIntent = await _onvoPayService.CreatePaymentIntentAsync(
                    amount: request.Amount,
                    currency: request.Currency ?? "CRC",
                    description: description,
                    usuarioId: usuarioId,
                    autoId: request.AutoId,
                    anuncioPublicidadId: request.AnuncioPublicidadId,
                    metadata: metadata
                );

                if (paymentIntent == null)
                {
                    return StatusCode(500, new { error = "No se pudo crear la sesión de pago" });
                }

                return Ok(new
                {
                    paymentIntentId = paymentIntent.id,
                    amount = paymentIntent.amount,
                    currency = paymentIntent.currency,
                    status = paymentIntent.status,
                    description = paymentIntent.description,
                    publishableKey = _onvoPayService.GetPublishableKey()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Payment Intent");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el estado de un pago
        /// </summary>
        [HttpGet("status/{paymentIntentId}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
        {
            try
            {
                var pago = await _onvoPayService.GetPagoByPaymentIntentIdAsync(paymentIntentId);
                
                if (pago == null)
                {
                    return NotFound(new { error = "Pago no encontrado" });
                }

                return Ok(new
                {
                    paymentIntentId = pago.PaymentIntentId,
                    status = pago.Status,
                    amount = pago.Amount,
                    currency = pago.Currency,
                    description = pago.Description,
                    createdAt = pago.CreatedAt,
                    completedAt = pago.CompletedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado del pago");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene la llave pública de ONVO para el frontend
        /// </summary>
        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            return Ok(new
            {
                publishableKey = _onvoPayService.GetPublishableKey()
            });
        }
        
        /// <summary>
        /// Verifica si una placa ya existe en la base de datos
        /// </summary>
        [HttpGet("verificar-placa/{placa}")]
        public IActionResult VerificarPlaca(string placa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(placa))
                {
                    return BadRequest(new { error = "La placa es requerida" });
                }
                
                var existe = _context.Autos.Any(a => a.PlacaVehiculo == placa);
                
                return Ok(new { existe = existe });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar placa");
                return StatusCode(500, new { error = "Error al verificar la placa" });
            }
        }

        /// <summary>
        /// Cancela un auto pendiente de pago (elimina el auto si está inactivo)
        /// </summary>
        [HttpPost("cancelar-auto/{autoId}")]
        public async Task<IActionResult> CancelarAuto(int autoId)
        {
            try
            {
                var auto = await _context.Autos.FindAsync(autoId);
                
                if (auto == null)
                {
                    // Si no existe, consideramos exitoso (ya fue eliminado)
                    return Ok(new { success = true, message = "Auto no encontrado o ya eliminado" });
                }

                // Solo eliminar si el auto está inactivo (pendiente de pago)
                if (!auto.Activo)
                {
                    // Primero eliminar registros relacionados para evitar FK constraint errors
                    
                    // 1. Eliminar pagos asociados
                    var pagosAsociados = await _context.PagosOnvo
                        .Where(p => p.AutoId == autoId)
                        .ToListAsync();
                    if (pagosAsociados.Any())
                    {
                        _context.PagosOnvo.RemoveRange(pagosAsociados);
                        _logger.LogInformation("Eliminados {Count} pagos asociados al auto {AutoId}", pagosAsociados.Count, autoId);
                    }
                    
                    // 2. Eliminar solicitudes de pre-aprobación asociadas
                    var solicitudesAsociadas = await _context.SolicitudesPreAprobacion
                        .Where(s => s.AutoId == autoId)
                        .ToListAsync();
                    if (solicitudesAsociadas.Any())
                    {
                        _context.SolicitudesPreAprobacion.RemoveRange(solicitudesAsociadas);
                        _logger.LogInformation("Eliminadas {Count} solicitudes pre-aprobación asociadas al auto {AutoId}", solicitudesAsociadas.Count, autoId);
                    }
                    
                    // 3. Eliminar favoritos asociados
                    var favoritosAsociados = await _context.Favoritos
                        .Where(f => f.AutoId == autoId)
                        .ToListAsync();
                    if (favoritosAsociados.Any())
                    {
                        _context.Favoritos.RemoveRange(favoritosAsociados);
                        _logger.LogInformation("Eliminados {Count} favoritos asociados al auto {AutoId}", favoritosAsociados.Count, autoId);
                    }
                    
                    // Ahora sí eliminar el auto
                    _context.Autos.Remove(auto);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Auto {AutoId} cancelado y eliminado por usuario", autoId);
                    return Ok(new { success = true, message = "Anuncio cancelado correctamente" });
                }
                else
                {
                    return BadRequest(new { error = "No se puede cancelar un anuncio activo" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar auto {AutoId}", autoId);
                return StatusCode(500, new { error = "Error al cancelar el anuncio: " + ex.Message });
            }
        }
        
        /// <summary>
        /// Activa un auto después de un pago exitoso
        /// Si el auto tiene PlanVisibilidad = 5 (gratuito), envía a pre-aprobación en lugar de activar
        /// </summary>
        [HttpPost("activar-auto/{autoId}")]
        public async Task<IActionResult> ActivarAuto(int autoId)
        {
            try
            {
                _logger.LogInformation("[ACTIVAR-AUTO] Iniciando activación de auto ID: {AutoId}", autoId);
                Console.WriteLine($"[ACTIVAR-AUTO] Iniciando activación de auto ID: {autoId}");
                
                var auto = await _context.Autos.FindAsync(autoId);
                
                if (auto == null)
                {
                    _logger.LogWarning("[ACTIVAR-AUTO] Auto {AutoId} no encontrado", autoId);
                    return NotFound(new { error = "Auto no encontrado" });
                }

                Console.WriteLine($"[ACTIVAR-AUTO] Auto encontrado. PlanVisibilidad: {auto.PlanVisibilidad}, Activo antes: {auto.Activo}");
                
                // Si es plan gratuito (5), crear solicitud de pre-aprobación en lugar de activar directamente
                if (auto.PlanVisibilidad == 5)
                {
                    // Verificar si ya existe una solicitud pendiente
                    var solicitudExistente = await _context.SolicitudesPreAprobacion
                        .FirstOrDefaultAsync(s => s.AutoId == autoId && !s.Procesada);
                    
                    if (solicitudExistente == null)
                    {
                        // Obtener datos del usuario
                        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == auto.EmailPropietario);
                        
                        var solicitud = new AutoClick.Models.SolicitudPreAprobacion
                        {
                            Nombre = usuario?.Nombre ?? "Usuario",
                            Apellidos = usuario?.Apellidos ?? "AutoClick",
                            Telefono = usuario?.NumeroTelefono ?? "No proporcionado",
                            Email = auto.EmailPropietario ?? "",
                            AutoId = auto.Id,
                            FechaSolicitud = DateTime.Now,
                            Procesada = false,
                            Notas = $"Solicitud de aprobación (con banderines pagados): {auto.Marca} {auto.Modelo} {auto.Ano}, Placa: {auto.PlacaVehiculo}"
                        };
                        
                        _context.SolicitudesPreAprobacion.Add(solicitud);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("[ACTIVAR-AUTO] Auto {AutoId} con plan gratuito enviado a pre-aprobación. Solicitud ID: {SolicitudId}", autoId, solicitud.Id);
                        Console.WriteLine($"[ACTIVAR-AUTO] Auto {autoId} enviado a pre-aprobación (plan gratuito con banderines). Solicitud ID: {solicitud.Id}");
                    }
                    else
                    {
                        _logger.LogInformation("[ACTIVAR-AUTO] Auto {AutoId} ya tiene solicitud de pre-aprobación pendiente", autoId);
                    }
                    
                    return Ok(new { success = true, message = "Pago procesado. Tu anuncio está pendiente de aprobación.", requiresApproval = true });
                }
                
                // Para otros planes (1-4), activar el auto directamente
                auto.Activo = true;
                
                // Marcar explícitamente como modificado para asegurar que EF detecte el cambio
                _context.Entry(auto).State = EntityState.Modified;
                
                Console.WriteLine($"[ACTIVAR-AUTO] Auto.Activo establecido a: {auto.Activo}, Estado: {_context.Entry(auto).State}");
                
                var cambios = await _context.SaveChangesAsync();
                
                Console.WriteLine($"[ACTIVAR-AUTO] Cambios guardados: {cambios} registros afectados");
                _logger.LogInformation("[ACTIVAR-AUTO] Auto {AutoId} activado. Registros afectados: {Cambios}", autoId, cambios);
                
                return Ok(new { success = true, message = "Anuncio activado correctamente", cambios = cambios });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ACTIVAR-AUTO] Error al activar auto {AutoId}", autoId);
                Console.WriteLine($"[ACTIVAR-AUTO] EXCEPCIÓN: {ex.Message}");
                return StatusCode(500, new { error = "Error al activar el anuncio: " + ex.Message });
            }
        }
    }

    // DTOs
    public class CreatePaymentRequest
    {
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public int? AutoId { get; set; }
        public int? AnuncioPublicidadId { get; set; }
        public string? OrderReference { get; set; }
    }
}
