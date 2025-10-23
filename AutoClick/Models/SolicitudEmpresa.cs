using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    /// <summary>
    /// Representa una solicitud de empresa para anunciarse en AutoClick.cr
    /// </summary>
    public class SolicitudEmpresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreEmpresa { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string RepresentanteLegal { get; set; } = "";

        [Required]
        [MaxLength(50)]
        public string Industria { get; set; } = "";

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; } = "";

        [Required]
        [MaxLength(1000)]
        public string DescripcionEmpresa { get; set; } = "";

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Estados posibles: Pendiente, EnRevision, Contactado, Aprobado, Rechazado
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";

        /// <summary>
        /// Notas internas para el equipo de AutoClick
        /// </summary>
        [MaxLength(500)]
        public string? NotasInternas { get; set; }
    }
}
