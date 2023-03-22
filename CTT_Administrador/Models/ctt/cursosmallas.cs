using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class cursosmallas
{
    public int idCursoMalla { get; set; }

    public int? idCurso { get; set; }

    public int? idCursoAsociado { get; set; }

    public decimal? valor { get; set; }

    public sbyte? activo { get; set; }

    public virtual cursos? idCursoAsociadoNavigation { get; set; }

    public virtual cursos? idCursoNavigation { get; set; }
}
