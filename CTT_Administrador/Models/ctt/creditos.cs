using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class creditos
{
    [Key]
    public int idCredito { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaCredito { get; set; }

    public int? idMatricula { get; set; }

    public sbyte? activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaDesactivacion { get; set; }
}
