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
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }
    
    [Required]
    [MaxLength(5)]
    [Display(Name = "Divisa")]
    public string Divisa { get; set; } = "CRC";
    
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
    
    [MaxLength(50)]
    [Display(Name = "Cantón")]
    public string? Canton { get; set; }
    
    [MaxLength(200)]
    [Display(Name = "Ubicación Exacta")]
    public string UbicacionExacta { get; set; } = string.Empty;
    
    // Descripción del vehículo
    [Column(TypeName = "TEXT")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;
    
    // Configuración del anuncio
    [Required]
    [Range(0, 5)]
    [Display(Name = "Plan de Visibilidad")]
    public int PlanVisibilidad { get; set; } = 0; // 0 = Pendiente de aprobación, 1+ = Aprobado/Pagado
    
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
    
    [MaxLength(20)]
    [Display(Name = "Estado Agenda Fotografía")]
    public string EstadoAgendaFotografia { get; set; } = "Sin agendar"; // "Sin agendar" o "Agendado"
    
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
        $"{UbicacionExacta}, {(!string.IsNullOrEmpty(Canton) ? Canton + ", " : "")}{Provincia}" : 
        $"{(!string.IsNullOrEmpty(Canton) ? Canton + ", " : "")}{Provincia}";
    
    [NotMapped]
    [Display(Name = "Precio Formateado")]
    public string PrecioFormateado => 
        Divisa == "USD" ? $"${Precio:N0}" : $"₡{Precio:N0}";
    
    [NotMapped]
    [Display(Name = "Precio en CRC")]
    public string PrecioEnCRC => 
        AutoClick.Helpers.PrecioHelper.FormatearPrecioEnCRC(Precio, Divisa);
    
    // Métodos helper para manejar los arrays JSON
    [NotMapped]
    public List<string> ExtrasExteriorList
    {
        get => SafeJsonDeserialize(ExtrasExterior);
        set => ExtrasExterior = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasInteriorList
    {
        get => SafeJsonDeserialize(ExtrasInterior);
        set => ExtrasInterior = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasMultimediaList
    {
        get => SafeJsonDeserialize(ExtrasMultimedia);
        set => ExtrasMultimedia = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasSeguridadList
    {
        get => SafeJsonDeserialize(ExtrasSeguridad);
        set => ExtrasSeguridad = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasRendimientoList
    {
        get => SafeJsonDeserialize(ExtrasRendimiento);
        set => ExtrasRendimiento = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ExtrasAntiRoboList
    {
        get => SafeJsonDeserialize(ExtrasAntiRobo);
        set => ExtrasAntiRobo = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> ImagenesUrlsList
    {
        get => SafeJsonDeserialize(ImagenesUrls);
        set => ImagenesUrls = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    [NotMapped]
    public List<string> VideosUrlsList
    {
        get => SafeJsonDeserialize(VideosUrls);
        set => VideosUrls = System.Text.Json.JsonSerializer.Serialize(value);
    }
    
    // Helper method for safe JSON deserialization
    private List<string> SafeJsonDeserialize(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return new List<string>();
            
        // Si no comienza con '[' no es un array JSON válido
        if (!jsonString.Trim().StartsWith('['))
            return new List<string>();
            
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(jsonString) ?? new List<string>();
        }
        catch (System.Text.Json.JsonException)
        {
            // Si falla la deserialización, retorna lista vacía
            return new List<string>();
        }
    }
    
    // Helper method for title case formatting
    private string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }

    // Helper method to get the tag video file based on BanderinAdquirido
    [NotMapped]
    public string? BanderinVideoUrl
    {
        get
        {
            if (BanderinAdquirido <= 0) return null;

            var tagFiles = new Dictionary<int, string>
            {
                { 1, "Versión Americana.gif" },
                { 2, "ÚNICO DUEÑO.gif" },
                { 3, "FULL EXTRAS-.gif" },
                { 4, "MANTENIMIENTO DE AGENCIA-.gif" },
                { 5, "PERFECTO ESTADO.gif" },
                { 6, "AL DÍA.gif" },
                { 7, "BAJO KILOMETRAJE.gif" },
                { 8, "NEGOCIABLE.gif" },
                { 9, "FINANCIAMIENTO DISPONIBLE.gif" },
                { 10, "ESCUCHO OFERTAS.gif" },
                { 11, "IMPECABLE.gif" },
                { 12, "CERO DETALLES.gif" },
                { 13, "POCO USO.gif" },
                { 14, "REGISTRO LIMPIO.gif" },
                { 15, "TRASPASO INCLUÍDO.gif" },
                { 16, "RECIBO.gif" },
                { 17, "VENDO O CAMBIO.gif" },
                { 18, "URGE VENDER.gif" },
                { 19, "PRECIO ESPECIAL.gif" },
                { 20, "OPORTUNIDAD UNICA.gif" },
                { 21, "LLAME AHORA.gif" },
                { 22, "ÚNICO EN EL PAÍS.gif" },
                { 23, "EDICIÓN LIMITADA.gif" },
                { 24, "PARA INSCRIBIR.gif" },
                { 25, "BAJO CONSUMO.gif" },
                { 26, "GARANTÍA EXTENDIDA.gif" },
                { 27, "FULL PPF.gif" },
                { 28, "TRATAMIENTO CERÁMICO.gif" },
                { 29, "TAYLOR MADE.gif" },
                { 30, "CEDO DEUDA.gif" }
            };

            if (!tagFiles.ContainsKey(BanderinAdquirido))
                return null;
                
            // Simplemente devolver el nombre del archivo
            // La URL completa se manejará en el PageModel usando BanderinesService
            return tagFiles[BanderinAdquirido];
        }
    }
    
    // Propiedad para recibir archivos del formulario (no se mapea a BD)
    [NotMapped]
    [Display(Name = "Fotos del Vehículo")]
    public List<IFormFile>? Fotos { get; set; }
}