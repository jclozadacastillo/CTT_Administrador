using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class tiposcursos
{
    [Key]
    public int idTipoCurso { get; set; }

    [StringLength(60)]
    public string? tipoCurso { get; set; }

    public sbyte? esDiplomado { get; set; }

    public sbyte? esCurso { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idTipoCursoNavigation")]
    public virtual ICollection<cursos> cursos { get; set; } = new List<cursos>();
}
