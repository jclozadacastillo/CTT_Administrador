using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class asignacionesinstructorescalificaciones
{
    public int idAsignacion { get; set; }

    public int? idGrupoCurso { get; set; }

    public int? idCurso { get; set; }

    public int? idInstructor { get; set; }

    public string? paralelo { get; set; }

    public DateTime? fechaLimiteNotas { get; set; }

    public sbyte? atrasoNotas { get; set; }

    public DateTime? fechaLimiteNotasAtraso { get; set; }

    public DateTime? fechaRegistro { get; set; }

    public string? usuarioRegistra { get; set; }

    public sbyte? activo { get; set; }

    public string? observacion { get; set; }
}
