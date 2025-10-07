using System;
using System.Collections.Generic;

namespace AutoClick.TempModels;

public partial class Usuario
{
    public string Email { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? NombreAgencia { get; set; }

    public string NumeroTelefono { get; set; } = null!;

    public virtual ICollection<Auto> Autos { get; set; } = new List<Auto>();
}
