using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    /// <summary>
    /// Solicitud de pre-aprobación de financiamiento
    /// </summary>
    [Table("SolicitudPreAprobacion")]
    public class SolicitudPreAprobacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Display(Name = "Número de Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ID del Vehículo")]
        public int AutoId { get; set; }

        [Display(Name = "Fecha de Solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Display(Name = "Procesada")]
        public bool Procesada { get; set; } = false;

        [Display(Name = "Aprobada")]
        public bool? Aprobada { get; set; }

        [Display(Name = "Fecha de Procesamiento")]
        public DateTime? FechaProcesamiento { get; set; }

        [MaxLength(500)]
        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        // Navigation property
        [ForeignKey("AutoId")]
        public virtual Auto? Auto { get; set; }
    }
}
