using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoClick.Models;

public class Auto
{
    [Key]
    public int Id { get; set; }
    
    // Información básica del vehículo
    [Required]
    [MaxLength(50)]
    [Display(Name = "Marca")]
    public string Marca { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = string.Empty;
    
    [Required]
    [Range(1900, 2030)]
    [Display(Name = "Año")]
    public int Ano { get; set; }
    
    [MaxLength(20)]
    [Display(Name = "Placa del Vehículo")]
    public string? PlacaVehiculo { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    [Display(Name = "Valor Fiscal")]
    public decimal ValorFiscal { get; set; }
    
    // Especificaciones técnicas
    [Required]
    [MaxLength(50)]
    [Display(Name = "Carrocería")]
    public string Carroceria { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    [Display(Name = "Combustible")]
    public string Combustible { get; set; } = string.Empty;
    
    [MaxLength(20)]
    [Display(Name = "Cilindrada")]
    public string Cilindrada { get; set; } = string.Empty;
    
    // Colores
    [MaxLength(30)]
    [Display(Name = "Color Exterior")]
    public string ColorExterior { get; set; } = string.Empty;
    
    [MaxLength(30)]
    [Display(Name = "Color Interior")]
    public string ColorInterior { get; set; } = string.Empty;
    
    // Características físicas
    [Range(2, 6)]
    [Display(Name = "Número de Puertas")]
    public int NumeroPuertas { get; set; }
    
    [Range(1, 50)]
    [Display(Name = "Número de Pasajeros")]
    public int NumeroPasajeros { get; set; }
    
    [MaxLength(30)]
    [Display(Name = "Transmisión")]
    public string Transmision { get; set; } = string.Empty;
    
    [MaxLength(30)]
    [Display(Name = "Tracción")]
    public string Traccion { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    [Display(Name = "Kilometraje")]
    public int Kilometraje { get; set; }
    
    [Required]
    [MaxLength(30)]
    [Display(Name = "Condición")]
    public string Condicion { get; set; } = string.Empty;
    
    // Extras (almacenados como JSON strings)
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Exterior")]
    public string ExtrasExterior { get; set; } = "[]"; // JSON array
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Interior")]
    public string ExtrasInterior { get; set; } = "[]"; // JSON array
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Multimedia")]
    public string ExtrasMultimedia { get; set; } = "[]"; // JSON array
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Seguridad")]
    public string ExtrasSeguridad { get; set; } = "[]"; // JSON array
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Rendimiento")]
    public string ExtrasRendimiento { get; set; } = "[]"; // JSON array
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "Extras Anti-robo")]
    public string ExtrasAntiRobo { get; set; } = "[]"; // JSON array
    
    // Ubicación
    [Required]
    [MaxLength(50)]
    [Display(Name = "Provincia")]
    public string Provincia { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Cantón")]
    public string Canton { get; set; } = string.Empty;
    
    [MaxLength(200)]
    [Display(Name = "Ubicación Exacta")]
    public string UbicacionExacta { get; set; } = string.Empty;
    
    // Configuración del anuncio
    [Required]
    [Range(1, 5)]
    [Display(Name = "Plan de Visibilidad")]
    public int PlanVisibilidad { get; set; } = 1;
    
    [Range(0, 10)]
    [Display(Name = "Banderín Adquirido")]
    public int BanderinAdquirido { get; set; } = 0;
    
    // Multimedia Storage - Referencias a archivos en blob storage
    [Column(TypeName = "TEXT")]
    [Display(Name = "URLs de Imágenes")]
    public string ImagenesUrls { get; set; } = "[]"; // JSON array de URLs
    
    [Column(TypeName = "TEXT")]
    [Display(Name = "URLs de Videos")]
    public string VideosUrls { get; set; } = "[]"; // JSON array de URLs
    
    [MaxLength(500)]
    [Display(Name = "Imagen Principal")]
    public string ImagenPrincipal { get; set; } = string.Empty;
    
    // Relación con Usuario propietario
    [Required]
    [MaxLength(150)]
    [Display(Name = "Email del Propietario")]
    public string EmailPropietario { get; set; } = string.Empty;
    
    // Fechas de auditoría
    [Display(Name = "Fecha de Creación")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    [Display(Name = "Fecha de Actualización")]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    
    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
    
    // Navigation property
    [ForeignKey("EmailPropietario")]
    public virtual Usuario? Propietario { get; set; }
    
    // Propiedades computadas para facilitar el uso
    [NotMapped]
    [Display(Name = "Nombre Completo")]
    public string NombreCompleto => $"{Ano} {ToTitleCase(Marca)} {ToTitleCase(Modelo)}";
    
    [NotMapped]
    [Display(Name = "Ubicación Completa")]
    public string UbicacionCompleta => 
        !string.IsNullOrEmpty(UbicacionExacta) ? 
        $"{UbicacionExacta}, {Canton}, {Provincia}" : 
        $"{Canton}, {Provincia}";
    
    [NotMapped]
    [Display(Name = "Valor Fiscal Formateado")]
    public string ValorFiscalFormateado => 
        $"₡{ValorFiscal:N0}"; // Formato para colones costarricenses
    
    // Métodos helper para manejar los arrays JSON
    [NotMapped]
    public List<string> ExtrasExteriorList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasExterior) ?? new List<string>();
        set => ExtrasExterior = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasInteriorList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasInterior) ?? new List<string>();
        set => ExtrasInterior = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasMultimediaList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasMultimedia) ?? new List<string>();
        set => ExtrasMultimedia = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasSeguridadList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasSeguridad) ?? new List<string>();
        set => ExtrasSeguridad = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasRendimientoList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasRendimiento) ?? new List<string>();
        set => ExtrasRendimiento = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasAntiRoboList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtrasAntiRobo) ?? new List<string>();
        set => ExtrasAntiRobo = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ImagenesUrlsList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ImagenesUrls) ?? new List<string>();
        set => ImagenesUrls = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> VideosUrlsList
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(VideosUrls) ?? new List<string>();
        set => VideosUrls = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    // Helper method for title case formatting
    private string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }
}