using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class detallecreditos
{
    [Key]
    public int idDetalleCredito { get; set; }

    public int? idCredito { get; set; }

    public int? idCurso { get; set; }

    [Precision(10, 2)]
    public decimal? valor { get; set; }

    [Precision(10, 2)]
    public decimal? valorPendiente { get; set; }

    public sbyte? cancelado { get; set; }

    public sbyte? activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaDesactivacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaRegistro { get; set; }
}
