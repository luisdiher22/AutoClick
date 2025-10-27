using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class Usuarios
{
    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string NumeroTelefono { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string? NombreAgencia { get; set; }

    public bool EsAdministrador { get; set; }

    public virtual ICollection<Autos> Autos { get; set; } = new List<Autos>();

    public virtual ICollection<Favoritos> Favoritos { get; set; } = new List<Favoritos>();
}
