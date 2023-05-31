using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idRol", Name = "idRol")]
[Index("idUsuario", Name = "idUsuario")]
public partial class rolesusuarios
{
    [Key]
    public int idRolUsuario { get; set; }

    public int? idRol { get; set; }

    public int? idUsuario { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idRol")]
    [InverseProperty("rolesusuarios")]
    public virtual roles? idRolNavigation { get; set; }

    [ForeignKey("idUsuario")]
    [InverseProperty("rolesusuarios")]
    public virtual usuarios? idUsuarioNavigation { get; set; }
}
