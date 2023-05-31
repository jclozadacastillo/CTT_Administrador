using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class usuarios
{
    [Key]
    public int idUsuario { get; set; }

    [StringLength(100)]
    public string? nombre { get; set; }

    [StringLength(50)]
    public string? usuario { get; set; }

    [StringLength(50)]
    public string? clave { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idUsuarioNavigation")]
    public virtual ICollection<rolesusuarios> rolesusuarios { get; set; } = new List<rolesusuarios>();
}
