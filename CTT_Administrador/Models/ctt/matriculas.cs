using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class matriculas
{
    [Key]
    public int idMatricula { get; set; }

    public int? idEstudiante { get; set; }

    public int? idCliente { get; set; }

    public int? idGrupoCurso { get; set; }

    public int? idTipoDescuento { get; set; }

    [StringLength(1)]
    public string? paralelo { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? fechaRegistro { get; set; }

    public sbyte? esUniandes { get; set; }

    [StringLength(3)]
    public string? idCarrera { get; set; }

    [StringLength(3)]
    public string? idCentro { get; set; }

    [StringLength(20)]
    public string? usuarioRegistro { get; set; }
}
