using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class usuarios
{
    public int idUsuario { get; set; }

    public string? nombre { get; set; }

    public string? usuario { get; set; }

    public string? clave { get; set; }

    public sbyte? activo { get; set; }

    public virtual ICollection<rolesusuarios> rolesusuarios { get; set; } = new List<rolesusuarios>();
}
