using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class periodos
{
    [Key]
    public int idPeriodo { get; set; }

    [StringLength(100)]
    public string? detalle { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaInicio { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaFin { get; set; }

    public sbyte? activo { get; set; }
}
