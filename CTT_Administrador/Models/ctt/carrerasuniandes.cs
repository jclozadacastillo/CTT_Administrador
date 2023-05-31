using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class carrerasuniandes
{
    [Key]
    [StringLength(3)]
    public string idCarrera { get; set; } = null!;

    [StringLength(200)]
    public string? carrera { get; set; }

    [StringLength(20)]
    public string? especializacion { get; set; }

    public sbyte? activa { get; set; }
}
