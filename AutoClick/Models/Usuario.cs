using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models;

public class Usuario
{
    [Key]
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Apellidos")]
    public string Apellidos { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [MaxLength(20)]
    [Display(Name = "Número de Teléfono")]
    public string NumeroTelefono { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;
    
    [MaxLength(100)]
    [Display(Name = "Nombre de Agencia")]
    public string? NombreAgencia { get; set; }
    
    [MaxLength(50)]
    [Display(Name = "Cédula Jurídica/Física")]
    public string? CedulaJuridica { get; set; }
    
    [MaxLength(50)]
    [Display(Name = "Provincia")]
    public string? Provincia { get; set; }
    
    [MaxLength(50)]
    [Display(Name = "Cantón")]
    public string? Canton { get; set; }
    
    [MaxLength(500)]
    [Display(Name = "URL de Imagen de Perfil")]
    public string? ImagenPerfilUrl { get; set; }
    
    [MaxLength(500)]
    [Display(Name = "URL de Banner")]
    public string? ImagenBannerUrl { get; set; }
    
    [Display(Name = "Es Agencia")]
    public bool EsAgencia => !string.IsNullOrEmpty(NombreAgencia);
    
    [Display(Name = "Es Administrador")]
    public bool EsAdministrador { get; set; } = false;
    
    [MaxLength(500)]
    [Display(Name = "URL del Logo")]
    public string? LogoUrl { get; set; }
    
    // Propiedades computadas para facilitar el uso
    [NotMapped]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Nombre} {Apellidos}";
    
    [NotMapped]
    [Display(Name = "Nombre a Mostrar")]
    public string NombreAMostrar => EsAgencia ? NombreAgencia! : NombreCompleto;
}