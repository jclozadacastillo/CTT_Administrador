using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCurso", Name = "idCurso")]
public partial class temas
{
    [Key]
    public int idTema { get; set; }

    public int? idCurso { get; set; }

    public sbyte? esTitulo { get; set; }

    [StringLength(500)]
    public string? tema { get; set; }

    public int? orden { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idCurso")]
    [InverseProperty("temas")]
    public virtual cursos? idCursoNavigation { get; set; }
}
