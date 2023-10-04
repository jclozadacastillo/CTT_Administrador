using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idBanco", Name = "idBanco")]
[Index("idTipoCuentaBanco", Name = "idTipoCuentaBanco")]
public partial class cuentasbancos
{
    [Key]
    public int idCuenta { get; set; }

    public int? idBanco { get; set; }

    public int? idTipoCuentaBanco { get; set; }

    [StringLength(50)]
    public string? alias { get; set; }

    [StringLength(20)]
    public string? numero { get; set; }

    [StringLength(20)]
    public string? codigo_financiero { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idBanco")]
    [InverseProperty("cuentasbancos")]
    public virtual bancos? idBancoNavigation { get; set; }

    [ForeignKey("idTipoCuentaBanco")]
    [InverseProperty("cuentasbancos")]
    public virtual tiposcuentasbancos? idTipoCuentaBancoNavigation { get; set; }

    [InverseProperty("idCuentaNavigation")]
    public virtual ICollection<pagosmatriculas> pagosmatriculas { get; set; } = new List<pagosmatriculas>();
}
