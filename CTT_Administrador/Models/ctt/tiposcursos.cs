using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class tiposcursos
{
    public int idTipoCurso { get; set; }

    public string? tipoCurso { get; set; }

    public sbyte? esDiplomado { get; set; }

    public sbyte? esCurso { get; set; }

    public sbyte? activo { get; set; }

    public virtual ICollection<cursos> cursos { get; } = new List<cursos>();
}
