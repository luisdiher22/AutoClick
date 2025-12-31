using AutoClick.Data;
using AutoClick.Services;
using Microsoft.AspNetCore.Mvc;
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
