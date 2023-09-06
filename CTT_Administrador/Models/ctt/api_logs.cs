using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class api_logs
{
    [Key]
    public int idLog { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaRegistro { get; set; }

    [Column(TypeName = "text")]
    public string? detalle { get; set; }

    public sbyte? error { get; set; }

    [StringLength(50)]
    public string? idUsuario { get; set; }
}
