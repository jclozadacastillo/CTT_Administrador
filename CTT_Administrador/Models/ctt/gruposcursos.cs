using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class gruposcursos
{
    [Key]
    public int idGrupoCurso { get; set; }

    public int? idPeriodo { get; set; }

    public int? idCurso { get; set; }

    public int? idModalidad { get; set; }

    [StringLength(100)]
    public string? horario { get; set; }

    public DateOnly? fechaInicioMatricula { get; set; }

    public DateOnly? fechaFinMatricula { get; set; }

    public DateOnly? fechaInicioCurso { get; set; }

    public DateOnly? fechaFinCurso { get; set; }

    public int? cupoMinimo { get; set; }

    public int? cupoMaximo { get; set; }

    public sbyte? esVisible { get; set; }

    public sbyte? activo { get; set; }
}
