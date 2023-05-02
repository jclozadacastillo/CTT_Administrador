using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class matriculas
{
    public int idMatricula { get; set; }

    public int? idEstudiante { get; set; }

    public int? idCliente { get; set; }

    public int? idGrupoCurso { get; set; }

    public int? idTipoDescuento { get; set; }

    public string? paralelo { get; set; }

    public DateTime? fechaRegistro { get; set; }

    public sbyte? esUniandes { get; set; }

    public string? idCarrera { get; set; }

    public string? idCentro { get; set; }

    public string? usuarioRegistro { get; set; }
}
