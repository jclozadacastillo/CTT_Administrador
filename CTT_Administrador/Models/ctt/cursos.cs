using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCategoria", Name = "idCategoria")]
[Index("idTipoCurso", Name = "idTipoCurso")]
public partial class cursos
{
    [Key]
    public int idCurso { get; set; }

    public int? idCategoria { get; set; }

    public int? idTipoCurso { get; set; }

    [StringLength(500)]
    public string curso { get; set; } = null!;

    [StringLength(100)]
    public string? imagen { get; set; }

    [Precision(10)]
    public decimal? precioCurso { get; set; }

    [StringLength(100)]
    public string? objetivoPrincipal { get; set; }

    [StringLength(1000)]
    public string? objetivoSecuncdario { get; set; }

    public sbyte? tienePrecedencia { get; set; }

    public int? idCursoPrecedencia { get; set; }

    public sbyte? activo { get; set; }

    public sbyte? esVisible { get; set; }

    public int? horasCurso { get; set; }

    public sbyte? procesoUniandes { get; set; }

    public int? puntajeMaximo { get; set; }

    public int? puntajeMinimo { get; set; }

    public sbyte? calificaAsistencia { get; set; }

    public int? asistenciaMinima { get; set; }

    public int? numeroNotas { get; set; }

    [InverseProperty("idCursoAsociadoNavigation")]
    public virtual ICollection<cursos_mallas> cursos_mallasidCursoAsociadoNavigation { get; set; } = new List<cursos_mallas>();

    [InverseProperty("idCursoNavigation")]
    public virtual ICollection<cursos_mallas> cursos_mallasidCursoNavigation { get; set; } = new List<cursos_mallas>();

    [ForeignKey("idCategoria")]
    [InverseProperty("cursos")]
    public virtual categorias? idCategoriaNavigation { get; set; }

    [ForeignKey("idTipoCurso")]
    [InverseProperty("cursos")]
    public virtual tiposcursos? idTipoCursoNavigation { get; set; }

    [InverseProperty("idCursoNavigation")]
    public virtual ICollection<temas> temas { get; set; } = new List<temas>();
}
