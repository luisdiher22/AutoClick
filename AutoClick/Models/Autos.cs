using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class Autos
{
    public int Id { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int Ano { get; set; }

    public string? PlacaVehiculo { get; set; }

    public decimal Precio { get; set; }

    public string Divisa { get; set; } = null!;

    public string Carroceria { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string Cilindrada { get; set; } = null!;

    public string ColorExterior { get; set; } = null!;

    public string ColorInterior { get; set; } = null!;

    public int NumeroPuertas { get; set; }

    public int NumeroPasajeros { get; set; }

    public string Transmision { get; set; } = null!;

    public string Traccion { get; set; } = null!;

    public int Kilometraje { get; set; }

    public string Condicion { get; set; } = null!;

    public string ExtrasExterior { get; set; } = null!;

    public string ExtrasInterior { get; set; } = null!;

    public string ExtrasMultimedia { get; set; } = null!;

    public string ExtrasSeguridad { get; set; } = null!;

    public string ExtrasRendimiento { get; set; } = null!;

    public string ExtrasAntiRobo { get; set; } = null!;

    public string Provincia { get; set; } = null!;

    public string Canton { get; set; } = null!;

    public string UbicacionExacta { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int PlanVisibilidad { get; set; }

    public int BanderinAdquirido { get; set; }

    public string ImagenesUrls { get; set; } = null!;

    public string VideosUrls { get; set; } = null!;

    public string ImagenPrincipal { get; set; } = null!;

    public string EmailPropietario { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public bool Activo { get; set; }

    public virtual Usuarios EmailPropietarioNavigation { get; set; } = null!;

    public virtual ICollection<Favoritos> Favoritos { get; set; } = new List<Favoritos>();
}
