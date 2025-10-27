using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class Mensajes
{
    public int Id { get; set; }

    public string? EmailCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string TipoConsulta { get; set; } = null!;

    public string Asunto { get; set; } = null!;

    public string ContenidoMensaje { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int Estado { get; set; }

    public string Prioridad { get; set; } = null!;

    public string? RespuestaAdmin { get; set; }

    public DateTime? FechaRespuesta { get; set; }

    public string? EmailAdminRespuesta { get; set; }
}
