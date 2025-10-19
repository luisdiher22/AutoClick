using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models;

public class Mensaje
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
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Tipo de Consulta")]
    public string TipoConsulta { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Asunto")]
    public string Asunto { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    [Display(Name = "Mensaje")]
    public string ContenidoMensaje { get; set; } = string.Empty;
    
    [MaxLength(20)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    [Display(Name = "Estado")]
    public EstadoMensaje Estado { get; set; } = EstadoMensaje.NoLeido;
    
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
        EstadoMensaje.NoLeido => "No Leído",
        EstadoMensaje.Leido => "Leído",
        EstadoMensaje.Respondido => "Respondido",
        EstadoMensaje.Archivado => "Archivado",
        _ => "Desconocido"
    };
    
    [NotMapped]
    [Display(Name = "Días Transcurridos")]
    public int DiasTranscurridos => (DateTime.UtcNow - FechaCreacion).Days;
    
    [NotMapped]
    [Display(Name = "Es Nuevo")]
    public bool EsNuevo => DiasTranscurridos <= 1 && Estado == EstadoMensaje.NoLeido;
    
    // Navigation properties (opcional - si queremos relacionar con Usuario)
    [ForeignKey("EmailCliente")]
    public virtual Usuario? Cliente { get; set; }
    
    [ForeignKey("EmailAdminRespuesta")]
    public virtual Usuario? AdminRespuesta { get; set; }
}

public enum EstadoMensaje
{
    NoLeido = 1,
    Leido = 2,
    Respondido = 3,
    Archivado = 4
}

public enum TipoConsultaMensaje
{
    InformacionGeneral,
    ConsultaComercial,
    SoporteTecnico,
    Sugerencias,
    Colaboraciones,
    Publicidad,
    Otro
}