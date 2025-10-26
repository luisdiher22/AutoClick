using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    public class AnuncioPublicidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmpresaPublicidadId { get; set; }

        [Required]
        [StringLength(500)]
        public string UrlImagen { get; set; } = string.Empty;

        [Required]
        public DateTime FechaPublicacion { get; set; }

        public int NumeroVistas { get; set; } = 0;

        public int NumeroClics { get; set; } = 0;

        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("EmpresaPublicidadId")]
        public virtual EmpresaPublicidad? EmpresaPublicidad { get; set; }
    }
}
