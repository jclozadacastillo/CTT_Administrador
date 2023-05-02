using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class periodos
{
    public int idPeriodo { get; set; }

    public string? detalle { get; set; }

    public DateTime? fechaInicio { get; set; }

    public DateTime? fechaFin { get; set; }

    public sbyte? activo { get; set; }
}
