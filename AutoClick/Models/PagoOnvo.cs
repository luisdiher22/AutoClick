using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    /// <summary>
    /// Representa un pago procesado a través de ONVO Pay
    /// </summary>
    public class PagoOnvo
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID del Payment Intent de ONVO
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PaymentIntentId { get; set; } = string.Empty;

        /// <summary>
    /// ID del auto/anuncio asociado al pago (para publicar el auto)
    /// </summary>
    public int? AutoId { get; set; }

    /// <summary>
    /// ID del anuncio de publicidad asociado al pago (para espacios publicitarios/carruseles)
        public int? AnuncioPublicidadId { get; set; }

        /// <summary>
        /// Monto del pago en centavos (USD) o céntimos (CRC)
        /// </summary>
        [Required]
        public int Amount { get; set; }

        /// <summary>
        /// Moneda del pago: USD o CRC
        /// </summary>
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Estado del pago: requires_confirmation, requires_payment_method, succeeded, failed, canceled, processing
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "requires_confirmation";

        /// <summary>
        /// Descripción del pago
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// ID del usuario que realiza el pago
        /// </summary>
        public int? UsuarioId { get; set; }

        /// <summary>
        /// Email del usuario que realiza el pago
        /// </summary>
        [StringLength(255)]
        public string? EmailUsuario { get; set; }

        /// <summary>
        /// Metadata adicional en formato JSON
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? Metadata { get; set; }

        /// <summary>
        /// Información del último error si el pago falló
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? LastPaymentError { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización del registro
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha en que el pago fue completado exitosamente
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Intentos de confirmación realizados
        /// </summary>
        public int ConfirmationAttempts { get; set; } = 0;

        // Navegación
        [ForeignKey("AutoId")]
        public virtual Auto? Auto { get; set; }

        [ForeignKey("AnuncioPublicidadId")]
        public virtual AnuncioPublicidad? AnuncioPublicidad { get; set; }

        // Nota: No se establece navegación directa con Usuario porque su PK es Email (string)
        // en lugar de Id (int). Se usa UsuarioId para tracking pero sin FK constraint.
    }
}
