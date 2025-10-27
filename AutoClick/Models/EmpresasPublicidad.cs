using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class EmpresasPublicidad
{
    public int Id { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaSalida { get; set; }

    public string EstadoCobros { get; set; } = null!;

    public bool Activa { get; set; }

    public virtual ICollection<AnunciosPublicidad> AnunciosPublicidad { get; set; } = new List<AnunciosPublicidad>();
}
