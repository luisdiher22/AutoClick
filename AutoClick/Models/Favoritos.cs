using System;
using System.Collections.Generic;

namespace AutoClick.Models;

public partial class Favoritos
{
    public int Id { get; set; }

    public string EmailUsuario { get; set; } = null!;

    public int AutoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Autos Auto { get; set; } = null!;

    public virtual Usuarios EmailUsuarioNavigation { get; set; } = null!;
}
