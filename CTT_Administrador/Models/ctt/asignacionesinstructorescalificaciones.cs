using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class asignacionesinstructorescalificaciones
{
    [Key]
    public int idAsignacion { get; set; }

    public int? idGrupoCurso { get; set; }

    public int? idCurso { get; set; }

    public int? idInstructor { get; set; }

    [StringLength(1)]
    public string? paralelo { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaLimiteNotas { get; set; }

    public sbyte? atrasoNotas { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaLimiteNotasAtraso { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? fechaRegistro { get; set; }

    [StringLength(20)]
    public string? usuarioRegistra { get; set; }

    public sbyte? activo { get; set; }

    [StringLength(100)]
    public string? observacion { get; set; }

    public sbyte? pasaFaltas { get; set; }
}
