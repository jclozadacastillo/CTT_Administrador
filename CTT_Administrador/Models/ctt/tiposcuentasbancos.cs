using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class tiposcuentasbancos
{
    [Key]
    public int idTipoCuentaBanco { get; set; }

    [StringLength(25)]
    public string? tipo { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idTipoCuentaBancoNavigation")]
    public virtual ICollection<cuentasbancos> cuentasbancos { get; set; } = new List<cuentasbancos>();
}
