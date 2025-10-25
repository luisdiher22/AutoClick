using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models;

public class Favorito
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(150)]
    [Display(Name = "Email del Usuario")]
    public string EmailUsuario { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "ID del Auto")]
    public int AutoId { get; set; }
    
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("EmailUsuario")]
    public virtual Usuario? Usuario { get; set; }
    
    [ForeignKey("AutoId")]
    public virtual Auto? Auto { get; set; }
}
