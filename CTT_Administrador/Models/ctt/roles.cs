using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class roles
{
    public int idRol { get; set; }

    public string? rol { get; set; }

    public string? nombre { get; set; }

    public sbyte? activo { get; set; }

    public virtual ICollection<rolesusuarios> rolesusuarios { get; } = new List<rolesusuarios>();
}
