using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class SolicitudesEmpresa
{
    public int Id { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public string RepresentanteLegal { get; set; } = null!;

    public string Industria { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string DescripcionEmpresa { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public string Estado { get; set; } = null!;

    public string? NotasInternas { get; set; }
}
