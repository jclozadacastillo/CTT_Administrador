using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class tiposdescuentos
{
    public int idTipoDescuento { get; set; }

    public string? nombreDescuento { get; set; }

    public decimal? porcentaje { get; set; }

    public sbyte? activo { get; set; }

    public sbyte? sinDescuento { get; set; }
}
