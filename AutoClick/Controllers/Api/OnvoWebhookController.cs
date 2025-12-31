using AutoClick.Data;
using AutoClick.Models;
using AutoClick.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AutoClick.Controllers.Api
{
    /// <summary>
    /// Controller para recibir y procesar webhooks de ONVO Pay
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OnvoWebhookController : ControllerBase
    {
        private readonly IOnvoPayService _onvoPayService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OnvoWebhookController> _logger;

        public OnvoWebhookController(
            IOnvoPayService onvoPayService,
            ApplicationDbContext context,
            ILogger<OnvoWebhookController> logger)
        {
            _onvoPayService = onvoPayService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para recibir webhooks de ONVO
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            string? webhookSecret = null;
            string? payload = null;
            WebhookEventOnvo? webhookLog = null;

            try
            {
                // Leer el payload del webhook
                using var reader = new StreamReader(Request.Body);
                payload = await reader.ReadToEndAsync();

                if (string.IsNullOrEmpty(payload))
                {
                    _logger.LogWarning("Webhook recibido sin payload");
                    return BadRequest(new { error = "Payload vacío" });
                }

                // Obtener el webhook secret del header
                if (Request.Headers.TryGetValue("X-Webhook-Secret", out var secretHeader))
                {
                    webhookSecret = secretHeader.ToString();
                }

                _logger.LogInformation("Webhook recibido de ONVO. Secret presente: {SecretPresent}", 
                    !string.IsNullOrEmpty(webhookSecret));

                // Deserializar el evento
                var webhookEvent = JsonSerializer.Deserialize<WebhookEvent>(payload,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                if (webhookEvent == null || webhookEvent.data == null)
                {
                    _logger.LogWarning("Webhook con formato inválido");
                    return BadRequest(new { error = "Formato de webhook inválido" });
                }

                // Crear registro del webhook
                webhookLog = new WebhookEventOnvo
                {
                    EventType = webhookEvent.type,
                    PaymentIntentId = webhookEvent.data.id,
                    Payload = payload,
                    WebhookSecret = webhookSecret,
                    ReceivedAt = DateTime.UtcNow,
                    Processed = false
                };

                _context.WebhookEventsOnvo.Add(webhookLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Procesando webhook {EventType} para Payment Intent {PaymentIntentId}",
                    webhookEvent.type, webhookEvent.data.id);

                // Procesar el evento según su tipo
                bool processed = false;

                switch (webhookEvent.type)
                {
                    case "payment-intent.succeeded":
                    case "payment-intent.failed":
                    case "payment-intent.deferred":
                        processed = await _onvoPayService.ProcessWebhookEventAsync(
                            webhookEvent.type,
                            webhookEvent.data,
                            webhookSecret ?? string.Empty
                        );
                        break;

                    case "checkout-session.succeeded":
                        // Manejar checkout session si es necesario
                        _logger.LogInformation("Checkout session succeeded recibido");
                        processed = true;
                        break;

                    case "mobile-transfer.received":
                        // Notificación informativa de SINPE, no requiere acción
                        _logger.LogInformation("Transferencia SINPE recibida (informativo)");
                        processed = true;
                        break;

                    default:
                        _logger.LogWarning("Tipo de evento no soportado: {EventType}", webhookEvent.type);
                        processed = false;
                        break;
                }

                // Actualizar registro del webhook
                webhookLog.Processed = processed;
                webhookLog.ProcessedAt = DateTime.UtcNow;

                if (!processed)
                {
                    webhookLog.ProcessingError = "No se pudo procesar el evento";
                }

                await _context.SaveChangesAsync();

                if (processed)
                {
                    _logger.LogInformation(
                        "Webhook procesado exitosamente: {EventType} - {PaymentIntentId}",
                        webhookEvent.type, webhookEvent.data.id);

                    // ONVO espera una respuesta 2xx para confirmar recepción
                    return Ok(new { received = true });
                }
                else
                {
                    _logger.LogError(
                        "Error procesando webhook: {EventType} - {PaymentIntentId}",
                        webhookEvent.type, webhookEvent.data.id);

                    return StatusCode(500, new { error = "Error procesando webhook" });
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializando payload del webhook");

                if (webhookLog != null)
                {
                    webhookLog.Processed = false;
                    webhookLog.ProcessingError = $"Error JSON: {ex.Message}";
                    webhookLog.ProcessedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return BadRequest(new { error = "Formato JSON inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado procesando webhook");

                if (webhookLog != null)
                {
                    webhookLog.Processed = false;
                    webhookLog.ProcessingError = $"Error: {ex.Message}";
                    webhookLog.ProcessedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Endpoint de health check para el webhook
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "ONVO Webhook Handler"
            });
        }

        /// <summary>
        /// Obtiene el historial de webhooks recibidos (para debugging)
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetWebhookHistory([FromQuery] int limit = 10)
        {
            try
            {
                var webhooks = await _context.WebhookEventsOnvo
                    .OrderByDescending(w => w.ReceivedAt)
                    .Take(Math.Min(limit, 100))
                    .Select(w => new
                    {
                        w.Id,
                        w.EventType,
                        w.PaymentIntentId,
                        w.Processed,
                        w.ProcessingError,
                        w.ReceivedAt,
                        w.ProcessedAt
                    })
                    .ToListAsync();

                return Ok(webhooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo historial de webhooks");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}
