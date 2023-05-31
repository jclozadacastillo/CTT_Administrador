using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class modalidades
{
    [Key]
    public int idModalidad { get; set; }

    [StringLength(30)]
    public string? modalidad { get; set; }

    public sbyte? activa { get; set; }
}
