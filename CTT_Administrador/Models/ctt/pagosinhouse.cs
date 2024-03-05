using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCuenta", Name = "idCuenta")]
[Index("idEstado", Name = "idEstado")]
[Index("idFormaPago", Name = "idFormaPago")]
[Index("idGrupoInHouse", Name = "idGrupoInHouse")]
public partial class pagosinhouse
{
    [Key]
    public int idPago { get; set; }

    public int? idGrupoInHouse { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaPago { get; set; }

    [StringLength(25)]
    public string? numeroFactura { get; set; }

    public int? idFormaPago { get; set; }

    public int? idCuenta { get; set; }

    [Precision(10)]
    public decimal? valor { get; set; }

    [StringLength(50)]
    public string? numeroComprobante { get; set; }

    [StringLength(200)]
    public string? imagenComprobante { get; set; }

    [StringLength(50)]
    public string? tarjetaMarca { get; set; }

    [StringLength(50)]
    public string? tarjetaAutorizacion { get; set; }

    [StringLength(50)]
    public string? tarjetaPedido { get; set; }

    [StringLength(200)]
    public string? observaciones { get; set; }

    public int? idEstado { get; set; }

    [ForeignKey("idCuenta")]
    [InverseProperty("pagosinhouse")]
    public virtual cuentasbancos? idCuentaNavigation { get; set; }

    [ForeignKey("idEstado")]
    [InverseProperty("pagosinhouse")]
    public virtual estadospagos? idEstadoNavigation { get; set; }

    [ForeignKey("idFormaPago")]
    [InverseProperty("pagosinhouse")]
    public virtual formaspagos? idFormaPagoNavigation { get; set; }

    [ForeignKey("idGrupoInHouse")]
    [InverseProperty("pagosinhouse")]
    public virtual gruposinhouse? idGrupoInHouseNavigation { get; set; }
}
