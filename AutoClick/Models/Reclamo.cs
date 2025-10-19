using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models;

public class Reclamo
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(150)]
    [EmailAddress]
    [Display(Name = "Correo Electrónico")]
    public string? EmailCliente { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;
    
    [MaxLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Tipo de Problema")]
    public string TipoProblema { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Asunto")]
    public string Asunto { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    [Display(Name = "Descripción del Problema")]
    public string Descripcion { get; set; } = string.Empty;
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    [Display(Name = "Estado")]
    public EstadoReclamo Estado { get; set; } = EstadoReclamo.Pendiente;
    
    [MaxLength(50)]
    [Display(Name = "Prioridad")]
    public string Prioridad { get; set; } = "Media";
    
    [MaxLength(1000)]
    [Display(Name = "Respuesta del Administrador")]
    public string? RespuestaAdmin { get; set; }
    
    [Display(Name = "Fecha de Respuesta")]
    public DateTime? FechaRespuesta { get; set; }
    
    [MaxLength(150)]
    [Display(Name = "Administrador que Respondió")]
    public string? EmailAdminRespuesta { get; set; }
    
    // Propiedades computadas
    [NotMapped]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Nombre} {Apellidos}";
    
    [NotMapped]
    [Display(Name = "Estado Formateado")]
    public string EstadoFormateado => Estado switch
    {
        EstadoReclamo.Pendiente => "Pendiente",
        EstadoReclamo.EnProceso => "En Proceso",
        EstadoReclamo.Resuelto => "Resuelto",
        EstadoReclamo.Cerrado => "Cerrado",
        _ => "Desconocido"
    };
    
    [NotMapped]
    [Display(Name = "Días Transcurridos")]
    public int DiasTranscurridos => (DateTime.UtcNow - FechaCreacion).Days;
    
    // Navigation properties (opcional - si queremos relacionar con Usuario)
    [ForeignKey("EmailCliente")]
    public virtual Usuario? Cliente { get; set; }
    
    [ForeignKey("EmailAdminRespuesta")]
    public virtual Usuario? AdminRespuesta { get; set; }
}

public enum EstadoReclamo
{
    Pendiente = 1,
    EnProceso = 2,
    Resuelto = 3,
    Cerrado = 4
}

public enum TipoProblemaReclamo
{
    ProblemasTecnicos,
    ProblemasConAnuncio,
    ProblemasConPago,
    ProblemasConCuenta,
    ReportarUsuario,
    SolicitudEliminacionCuenta,
    Otro
}