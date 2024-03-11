using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCliente", Name = "idCliente")]
[Index("idCuenta", Name = "idCuenta")]
[Index("idFormaPago", Name = "idFormaPago")]
[Index("idMatricula", Name = "idMatricula")]
public partial class pagosmatriculas
{
    [Key]
    public int idPagoMatricula { get; set; }

    public int? idMatricula { get; set; }

    [Column(TypeName = "date")]
    public DateTime? fechaPago { get; set; }

    [StringLength(25)]
    public string? numeroFactura { get; set; }

    public int? idFormaPago { get; set; }

    public int? idCuenta { get; set; }

    public int? idCliente { get; set; }

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

    [StringLength(10)]
    public string? usuarioValidacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaValidacion { get; set; }

    [ForeignKey("idCliente")]
    [InverseProperty("pagosmatriculas")]
    public virtual clientesfacturas? idClienteNavigation { get; set; }

    [ForeignKey("idCuenta")]
    [InverseProperty("pagosmatriculas")]
    public virtual cuentasbancos? idCuentaNavigation { get; set; }

    [ForeignKey("idFormaPago")]
    [InverseProperty("pagosmatriculas")]
    public virtual formaspagos? idFormaPagoNavigation { get; set; }

    [ForeignKey("idMatricula")]
    [InverseProperty("pagosmatriculas")]
    public virtual matriculas? idMatriculaNavigation { get; set; }
}
