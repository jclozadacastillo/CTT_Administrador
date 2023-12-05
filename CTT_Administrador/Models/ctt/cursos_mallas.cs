using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCurso", Name = "idCurso")]
[Index("idCursoAsociado", Name = "idCursoAsociado")]
public partial class cursos_mallas
{
    [Key]
    public int idCursoMalla { get; set; }

    public int? idCurso { get; set; }

    public int? idCursoAsociado { get; set; }

    [Precision(10, 2)]
    public decimal? valor { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idCursoAsociado")]
    [InverseProperty("cursos_mallasidCursoAsociadoNavigation")]
    public virtual cursos? idCursoAsociadoNavigation { get; set; }

    [ForeignKey("idCurso")]
    [InverseProperty("cursos_mallasidCursoNavigation")]
    public virtual cursos? idCursoNavigation { get; set; }
}
