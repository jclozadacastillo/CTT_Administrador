using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class gruposcursos
{
    public int idGrupoCurso { get; set; }

    public int? idPeriodo { get; set; }

    public int? idCurso { get; set; }

    public int? idModalidad { get; set; }

    public string? horario { get; set; }

    public DateTime? fechaInicioMatricula { get; set; }

    public DateTime? fechaFinMatricula { get; set; }

    public DateTime? fechaInicioCurso { get; set; }

    public DateTime? fechaFinCurso { get; set; }

    public int? cupoMinimo { get; set; }

    public int? cupoMaximo { get; set; }

    public sbyte? esVisible { get; set; }

    public sbyte? activo { get; set; }
}
