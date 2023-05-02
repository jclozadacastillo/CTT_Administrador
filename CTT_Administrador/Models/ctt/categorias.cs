using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class categorias
{
    public int idCategoria { get; set; }

    public string categoria { get; set; } = null!;

    public string? imagen { get; set; }

    public sbyte? activo { get; set; }

    public virtual ICollection<cursos> cursos { get; set; } = new List<cursos>();
}
