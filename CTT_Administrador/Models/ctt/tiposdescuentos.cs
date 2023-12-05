using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class tiposdescuentos
{
    [Key]
    public int idTipoDescuento { get; set; }

    [StringLength(100)]
    public string? nombreDescuento { get; set; }

    [Precision(10, 2)]
    public decimal? porcentaje { get; set; }

    public sbyte? activo { get; set; }

    public sbyte? sinDescuento { get; set; }
}
