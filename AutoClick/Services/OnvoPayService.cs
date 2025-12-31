using AutoClick.Data;
using AutoClick.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AutoClick.Services
{
    /// <summary>
    /// Servicio para integración con ONVO Pay
    /// </summary>
    public interface IOnvoPayService
    {
        Task<PaymentIntentResponse?> CreatePaymentIntentAsync(
            int amount,
            string currency,
            string description,
            int? usuarioId = null,
            int? autoId = null,
            int? anuncioPublicidadId = null,
            Dictionary<string, string>? metadata = null);

        Task<PaymentIntentResponse?> GetPaymentIntentAsync(string paymentIntentId);
        
        Task<PagoOnvo?> GetPagoByPaymentIntentIdAsync(string paymentIntentId);
        
        Task<bool> ProcessWebhookEventAsync(string eventType, PaymentIntentResponse paymentIntent, string webhookSecret);
        
        string GetPublishableKey();
    }

    public class OnvoPayService : IOnvoPayService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OnvoPaySettings _settings;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OnvoPayService> _logger;

        public OnvoPayService(
            IHttpClientFactory httpClientFactory,
            IOptions<OnvoPaySettings> settings,
            ApplicationDbContext context,
            ILogger<OnvoPayService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva Payment Intent en ONVO
        /// </summary>
        public async Task<PaymentIntentResponse?> CreatePaymentIntentAsync(
            int amount,
            string currency,
            string description,
            int? usuarioId = null,
            int? autoId = null,
            int? anuncioPublicidadId = null,
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                if (amount < 50) // Mínimo $0.50 USD
                {
                    throw new ArgumentException("El monto mínimo es $0.50 USD (50 centavos)");
                }

                var request = new CreatePaymentIntentRequest
                {
                    amount = amount,
                    currency = currency.ToUpper(),
                    description = description,
                    metadata = metadata ?? new Dictionary<string, string>()
                };

                // Agregar IDs a metadata
                if (autoId.HasValue)
                {
                    request.metadata["autoId"] = autoId.Value.ToString();
                }
                if (anuncioPublicidadId.HasValue)
                {
                    request.metadata["anuncioPublicidadId"] = anuncioPublicidadId.Value.ToString();
                }
                if (usuarioId.HasValue)
                {
                    request.metadata["usuarioId"] = usuarioId.Value.ToString();
                }

                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _settings.SecretKey);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Creando Payment Intent en ONVO para monto: {Amount} {Currency}", 
                    amount, currency);

                var response = await client.PostAsync("/v1/payment-intents", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al crear Payment Intent. Status: {Status}, Response: {Response}",
                        response.StatusCode, responseContent);
                    return null;
                }

                var paymentIntent = JsonSerializer.Deserialize<PaymentIntentResponse>(responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                if (paymentIntent != null)
                {
                    // Guardar en base de datos
                    var usuario = usuarioId.HasValue 
                        ? await _context.Usuarios.FindAsync(usuarioId.Value) 
                        : null;

                    var pago = new PagoOnvo
                    {
                        PaymentIntentId = paymentIntent.id,
                        Amount = paymentIntent.amount,
                        Currency = paymentIntent.currency,
                        Status = paymentIntent.status,
                        Description = paymentIntent.description,
                        UsuarioId = usuarioId,
                        EmailUsuario = usuario?.Email,
                        AutoId = autoId,
                        AnuncioPublicidadId = anuncioPublicidadId,
                        Metadata = JsonSerializer.Serialize(paymentIntent.metadata),
                        ConfirmationAttempts = paymentIntent.confirmationAttempts,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.PagosOnvo.Add(pago);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Payment Intent creado exitosamente. ID: {PaymentIntentId}", 
                        paymentIntent.id);
                }

                return paymentIntent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear Payment Intent en ONVO");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una Payment Intent de ONVO
        /// </summary>
        public async Task<PaymentIntentResponse?> GetPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

                var response = await client.GetAsync($"/v1/payment-intents/{paymentIntentId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error al obtener Payment Intent {PaymentIntentId}. Status: {Status}",
                        paymentIntentId, response.StatusCode);
                    return null;
                }

                var paymentIntent = JsonSerializer.Deserialize<PaymentIntentResponse>(responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                return paymentIntent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Payment Intent {PaymentIntentId}", paymentIntentId);
                return null;
            }
        }

        /// <summary>
        /// Obtiene un pago de la base de datos por Payment Intent ID
        /// </summary>
        public async Task<PagoOnvo?> GetPagoByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.PagosOnvo
                .Include(p => p.Auto)
                .Include(p => p.AnuncioPublicidad)
                .FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId);
        }

        /// <summary>
        /// Procesa un evento webhook de ONVO
        /// </summary>
        public async Task<bool> ProcessWebhookEventAsync(
            string eventType, 
            PaymentIntentResponse paymentIntent, 
            string webhookSecret)
        {
            try
            {
                // Validar webhook secret si está configurado
                if (!string.IsNullOrEmpty(_settings.WebhookSecret) && 
                    _settings.WebhookSecret != webhookSecret)
                {
                    _logger.LogWarning("Webhook secret inválido recibido");
                    return false;
                }

                // Buscar el pago en la base de datos
                var pago = await GetPagoByPaymentIntentIdAsync(paymentIntent.id);
                if (pago == null)
                {
                    _logger.LogWarning("Pago no encontrado para Payment Intent: {PaymentIntentId}", 
                        paymentIntent.id);
                    return false;
                }

                // Actualizar estado del pago
                pago.Status = paymentIntent.status;
                pago.ConfirmationAttempts = paymentIntent.confirmationAttempts;
                pago.UpdatedAt = DateTime.UtcNow;

                if (paymentIntent.lastPaymentError != null)
                {
                    pago.LastPaymentError = JsonSerializer.Serialize(paymentIntent.lastPaymentError);
                }

                // Procesar según tipo de evento
                switch (eventType)
                {
                    case "payment-intent.succeeded":
                        pago.Status = "succeeded";
                        pago.CompletedAt = DateTime.UtcNow;
                        
                        // Activar auto/anuncio del usuario
                        if (pago.AutoId.HasValue)
                        {
                            var auto = await _context.Autos.FindAsync(pago.AutoId.Value);
                            if (auto != null)
                            {
                                auto.Activo = true; // Activar el auto para que sea visible
                                // PlanVisibilidad ya fue establecido al crear el auto
                                _logger.LogInformation(
                                    "Auto {AutoId} activado por pago exitoso {PaymentIntentId}",
                                    auto.Id, paymentIntent.id);
                            }
                        }
                        
                        // Activar anuncio de publicidad (carruseles)
                        if (pago.AnuncioPublicidadId.HasValue)
                        {
                            var anuncio = await _context.AnunciosPublicidad
                                .FindAsync(pago.AnuncioPublicidadId.Value);
                            
                            if (anuncio != null)
                            {
                                anuncio.Activo = true;
                                anuncio.FechaPublicacion = DateTime.UtcNow;
                                
                                _logger.LogInformation(
                                    "Anuncio publicitario {AnuncioId} activado por pago exitoso {PaymentIntentId}",
                                    anuncio.Id, paymentIntent.id);
                            }
                        }
                        
                        _logger.LogInformation("Pago completado exitosamente: {PaymentIntentId}", 
                            paymentIntent.id);
                        break;

                    case "payment-intent.failed":
                        pago.Status = "failed";
                        _logger.LogWarning("Pago fallido: {PaymentIntentId}. Error: {Error}", 
                            paymentIntent.id, 
                            paymentIntent.lastPaymentError?.message);
                        break;

                    case "payment-intent.deferred":
                        pago.Status = "processing";
                        _logger.LogInformation("Pago en procesamiento (deferred): {PaymentIntentId}", 
                            paymentIntent.id);
                        break;

                    default:
                        _logger.LogWarning("Tipo de evento no manejado: {EventType}", eventType);
                        break;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando webhook event {EventType} para Payment Intent {PaymentIntentId}",
                    eventType, paymentIntent.id);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la llave pública para uso en el frontend
        /// </summary>
        public string GetPublishableKey()
        {
            return _settings.PublishableKey;
        }
    }
}
