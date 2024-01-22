using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[PrimaryKey("idMatricula", "idGrupoCurso", "idCurso")]
public partial class calificaciones
{
    [Key]
    public int idMatricula { get; set; }

    [Key]
    public int idGrupoCurso { get; set; }

    [Key]
    public int idCurso { get; set; }

    [Precision(5)]
    public decimal? nota1 { get; set; }

    [Precision(5)]
    public decimal? nota2 { get; set; }

    [Precision(5)]
    public decimal? nota3 { get; set; }

    [Precision(5)]
    public decimal? promedioFinal { get; set; }

    [Precision(5)]
    public decimal? faltas { get; set; }

    public sbyte? pierdeFaltas { get; set; }

    public sbyte? aprobado { get; set; }

    public sbyte? esExcento { get; set; }

    [StringLength(100)]
    public string? observacion { get; set; }

    [Precision(5)]
    public decimal? nota4 { get; set; }

    [Precision(5)]
    public decimal? nota5 { get; set; }

    [StringLength(10)]
    public string? justificacionObservacion { get; set; }

    public sbyte? justificaFaltas { get; set; }

    public sbyte? suspendido { get; set; }
}
