using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class calificaciones
{
    public int idMatricula { get; set; }

    public int idGrupoCurso { get; set; }

    public int idCurso { get; set; }

    public decimal? nota1 { get; set; }

    public decimal? nota2 { get; set; }

    public decimal? nota3 { get; set; }

    public decimal? promedioFinal { get; set; }

    public int? faltas { get; set; }

    public sbyte? pierdeFaltas { get; set; }

    public sbyte? aprobado { get; set; }

    public sbyte? esExcento { get; set; }

    public string? observacion { get; set; }
}
