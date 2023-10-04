using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class bancos
{
    [Key]
    public int idBanco { get; set; }

    [StringLength(100)]
    public string? banco { get; set; }

    [StringLength(20)]
    public string? codigo_financiero { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idBancoNavigation")]
    public virtual ICollection<cuentasbancos> cuentasbancos { get; set; } = new List<cuentasbancos>();
}
