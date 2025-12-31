namespace AutoClick.Services
{
    /// <summary>
    /// DTOs para la API de ONVO Pay
    /// </summary>
    /// 
    // Request para crear Payment Intent
    public class CreatePaymentIntentRequest
    {
        public int amount { get; set; }
        public string currency { get; set; } = "USD";
        public string? description { get; set; }
        public string? customerId { get; set; }
        public Dictionary<string, string>? metadata { get; set; }
    }

    // Response de Payment Intent
    public class PaymentIntentResponse
    {
        public string id { get; set; } = string.Empty;
        public int amount { get; set; }
        public int baseAmount { get; set; }
        public decimal exchangeRate { get; set; }
        public int capturableAmount { get; set; }
        public int receivedAmount { get; set; }
        public string? captureMethod { get; set; }
        public string currency { get; set; } = string.Empty;
        public DateTime createdAt { get; set; }
        public string? customerId { get; set; }
        public string? description { get; set; }
        public List<ChargeInfo>? charges { get; set; }
        public PaymentErrorInfo? lastPaymentError { get; set; }
        public string mode { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public DateTime updatedAt { get; set; }
        public Dictionary<string, string>? metadata { get; set; }
        public int confirmationAttempts { get; set; }
    }

    public class ChargeInfo
    {
        public string id { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public int amount { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class PaymentErrorInfo
    {
        public string? code { get; set; }
        public string? message { get; set; }
        public string? type { get; set; }
    }

    // Webhook Events
    public class WebhookEvent
    {
        public string type { get; set; } = string.Empty;
        public PaymentIntentResponse? data { get; set; }
    }

    // Respuesta de error de API
    public class OnvoApiError
    {
        public int statusCode { get; set; }
        public string? apiCode { get; set; }
        public List<string>? message { get; set; }
        public string? error { get; set; }
    }
}
