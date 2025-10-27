using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class AnunciosPublicidad
{
    public int Id { get; set; }

    public int EmpresaPublicidadId { get; set; }

    public string UrlImagen { get; set; } = null!;

    public DateTime FechaPublicacion { get; set; }

    public int NumeroVistas { get; set; }

    public int NumeroClics { get; set; }

    public bool Activo { get; set; }

    public string? UrlDestino { get; set; }

    public virtual EmpresasPublicidad EmpresaPublicidad { get; set; } = null!;
}
