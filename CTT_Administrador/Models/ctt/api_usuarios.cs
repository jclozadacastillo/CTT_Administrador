using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class api_usuarios
{
    [Key]
    [StringLength(50)]
    public string idUsuario { get; set; } = null!;

    [StringLength(50)]
    public string? clave { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaRegistro { get; set; }

    public sbyte? activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaDesactiva { get; set; }
}
