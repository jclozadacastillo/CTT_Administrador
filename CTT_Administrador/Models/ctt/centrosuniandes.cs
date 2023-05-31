using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Keyless]
public partial class centrosuniandes
{
    [StringLength(3)]
    public string? idCentro { get; set; }

    [StringLength(20)]
    public string? centro { get; set; }

    public sbyte? activo { get; set; }
}
