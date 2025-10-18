using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models
{
    public class VentaExterna
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? Link { get; set; }

        [MaxLength(100)]
        public string? Marca { get; set; }

        [MaxLength(100)]
        public string? Modelo { get; set; }

        public int? Año { get; set; }

        public int? Kilometraje { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrecioVenta { get; set; }

        [MaxLength(20)]
        public string? Placa { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ValorFiscal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PromedioValorMercado { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PromedioValorFiscal { get; set; }

        public DateTime FechaImportacion { get; set; } = DateTime.Now;
    }
}
