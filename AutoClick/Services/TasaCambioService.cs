using System.Text.Json;

namespace AutoClick.Services
{
    /// <summary>
    /// Interfaz para el servicio de consulta de tasa de cambio
    /// </summary>
    public interface ITasaCambioService
    {
        Task<decimal> ObtenerTasaCambioUSDaCRC();
    }

    /// <summary>
    /// Servicio para obtener la tasa de cambio del dólar en Costa Rica
    /// Consulta la API del Banco Central de Costa Rica (BCCR)
    /// </summary>
    public class TasaCambioService : ITasaCambioService
    {
        private readonly ILogger<TasaCambioService> _logger;
        private readonly HttpClient _httpClient;
        
        // Caché de la tasa de cambio
        private static decimal _tasaCacheada = 510m; // Valor por defecto
        private static DateTime _ultimaActualizacion = DateTime.MinValue;
        
        // La tasa se actualiza cada 24 horas
        private static readonly TimeSpan DURACION_CACHE = TimeSpan.FromHours(24);
        
        // Tasa de respaldo en caso de fallo de API
        private const decimal TASA_RESPALDO = 510m;

        public TasaCambioService(ILogger<TasaCambioService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10); // Timeout de 10 segundos
        }

        /// <summary>
        /// Obtiene la tasa de cambio de venta USD a CRC
        /// Utiliza caché de 24 horas para evitar llamadas excesivas
        /// </summary>
        public async Task<decimal> ObtenerTasaCambioUSDaCRC()
        {
            try
            {
                // Verificar si la caché aún es válida
                if (DateTime.Now - _ultimaActualizacion < DURACION_CACHE)
                {
                    _logger.LogInformation($"Usando tasa de cambio cacheada: {_tasaCacheada}");
                    return _tasaCacheada;
                }

                // Intentar obtener la tasa del BCCR
                var tasa = await ConsultarTasaBCCR();
                
                if (tasa > 0)
                {
                    _tasaCacheada = tasa;
                    _ultimaActualizacion = DateTime.Now;
                    
                    // Actualizar el helper estático para que todas las vistas usen la tasa actual
                    AutoClick.Helpers.PrecioHelper.ActualizarTasaCacheada(tasa);
                    
                    _logger.LogInformation($"Tasa de cambio actualizada desde API: {tasa}");
                    return tasa;
                }
                
                // Si falla, usar tasa cacheada o de respaldo
                _logger.LogWarning("No se pudo obtener tasa del BCCR, usando tasa cacheada o de respaldo");
                return _tasaCacheada > 0 ? _tasaCacheada : TASA_RESPALDO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tasa de cambio");
                return _tasaCacheada > 0 ? _tasaCacheada : TASA_RESPALDO;
            }
        }

        /// <summary>
        /// Consulta la tasa de cambio desde APIs públicas
        /// Prioridad: exchangerate-api.com (confiable y gratuito)
        /// </summary>
        private async Task<decimal> ConsultarTasaBCCR()
        {
            try
            {
                // OPCIÓN 1: exchangerate-api.com - API gratuita y muy confiable
                // Endpoint: https://api.exchangerate-api.com/v4/latest/USD
                var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    
                    if (data != null && data.ContainsKey("rates"))
                    {
                        var rates = data["rates"];
                        if (rates.TryGetProperty("CRC", out var crcRate))
                        {
                            if (decimal.TryParse(crcRate.GetDecimal().ToString(), 
                                System.Globalization.NumberStyles.Any, 
                                System.Globalization.CultureInfo.InvariantCulture, out var tasa))
                            {
                                // La API devuelve la tasa directa USD -> CRC
                                _logger.LogInformation($"Tasa obtenida de exchangerate-api: {tasa} CRC por USD");
                                return tasa;
                            }
                        }
                    }
                }
                
                _logger.LogWarning($"Respuesta no exitosa de API exchangerate-api: {response.StatusCode}");
                return 0;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al consultar tasa de cambio");
                return 0;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al consultar tasa de cambio");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al parsear respuesta de tasa de cambio");
                return 0;
            }
        }
    }
}
