using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    /// <summary>
    /// Registro de eventos webhook recibidos de ONVO
    /// </summary>
    public class WebhookEventOnvo
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tipo de evento: payment-intent.succeeded, payment-intent.failed, payment-intent.deferred, etc.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string EventType { get; set; } = string.Empty;

        /// <summary>
        /// ID del Payment Intent asociado
        /// </summary>
        [StringLength(100)]
        public string? PaymentIntentId { get; set; }

        /// <summary>
        /// Payload completo del webhook en formato JSON
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el webhook fue procesado exitosamente
        /// </summary>
        [Required]
        public bool Processed { get; set; } = false;

        /// <summary>
        /// Mensaje de error si el procesamiento falló
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? ProcessingError { get; set; }

        /// <summary>
        /// Fecha de recepción del webhook
        /// </summary>
        [Required]
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de procesamiento del webhook
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Secret recibido en el header X-Webhook-Secret
        /// </summary>
        [StringLength(500)]
        public string? WebhookSecret { get; set; }
    }
}
