using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class Reclamos
{
    public int Id { get; set; }

    public string? EmailCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public string TipoProblema { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public int Estado { get; set; }

    public string Prioridad { get; set; } = null!;

    public string? RespuestaAdmin { get; set; }

    public DateTime? FechaRespuesta { get; set; }

    public string? EmailAdminRespuesta { get; set; }
}
