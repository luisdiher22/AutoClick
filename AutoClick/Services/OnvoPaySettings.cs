namespace AutoClick.Services
{
    /// <summary>
    /// Configuración para el servicio de ONVO Pay
    /// </summary>
    public class OnvoPaySettings
    {
        public string BaseUrl { get; set; } = "https://api.onvopay.com";
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
