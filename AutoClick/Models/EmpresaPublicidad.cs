using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    public class EmpresaPublicidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreEmpresa { get; set; } = string.Empty;

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaSalida { get; set; }

        [Required]
        [StringLength(20)]
        public string EstadoCobros { get; set; } = "Al día"; // "Al día" o "Pendiente"

        public bool Activa { get; set; } = true;

        // Navegación
        public virtual ICollection<AnuncioPublicidad> Anuncios { get; set; } = new List<AnuncioPublicidad>();

        // Propiedades calculadas (no mapeadas a BD)
        [NotMapped]
        public int AnunciosActivos => Anuncios?.Count(a => a.Activo) ?? 0;

        [NotMapped]
        public int AnunciosInactivos => Anuncios?.Count(a => !a.Activo) ?? 0;

        [NotMapped]
        public int TotalAnuncios => Anuncios?.Count ?? 0;
    }
}
