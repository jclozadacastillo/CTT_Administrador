using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class roles
{
    [Key]
    public int idRol { get; set; }

    [StringLength(19)]
    public string? rol { get; set; }

    [StringLength(100)]
    public string? nombre { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idRolNavigation")]
    public virtual ICollection<rolesusuarios> rolesusuarios { get; set; } = new List<rolesusuarios>();
}
