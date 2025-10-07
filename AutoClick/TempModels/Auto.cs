using System;
using System.Collections.Generic;

namespace AutoClick.TempModels;

public partial class Auto
{
    public int Id { get; set; }

    public int Activo { get; set; }

    public int Ano { get; set; }

    public int BanderinAdquirido { get; set; }

    public string Canton { get; set; } = null!;

    public string Carroceria { get; set; } = null!;

    public string Cilindrada { get; set; } = null!;

    public string ColorExterior { get; set; } = null!;

    public string ColorInterior { get; set; } = null!;

    public string Combustible { get; set; } = null!;

    public string Condicion { get; set; } = null!;

    public string EmailPropietario { get; set; } = null!;

    public string ExtrasAntiRobo { get; set; } = null!;

    public string ExtrasExterior { get; set; } = null!;

    public string ExtrasInterior { get; set; } = null!;

    public string ExtrasMultimedia { get; set; } = null!;

    public string ExtrasRendimiento { get; set; } = null!;

    public string ExtrasSeguridad { get; set; } = null!;

    public DateTime FechaActualizacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string ImagenPrincipal { get; set; } = null!;

    public string ImagenesUrls { get; set; } = null!;

    public int Kilometraje { get; set; }

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public int NumeroPasajeros { get; set; }

    public int NumeroPuertas { get; set; }

    public string? PlacaVehiculo { get; set; }

    public int PlanVisibilidad { get; set; }

    public string Provincia { get; set; } = null!;

    public string Traccion { get; set; } = null!;

    public string Transmision { get; set; } = null!;

    public string UbicacionExacta { get; set; } = null!;

    public int ValorFiscal { get; set; }

    public string VideosUrls { get; set; } = null!;

    public virtual Usuario EmailPropietarioNavigation { get; set; } = null!;
}
