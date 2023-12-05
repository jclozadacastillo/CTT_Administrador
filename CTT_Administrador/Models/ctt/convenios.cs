using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idCliente", Name = "idCliente")]
public partial class convenios
{
    [Key]
    public int idConvenio { get; set; }

    public int? idCliente { get; set; }

    public DateOnly? fechaInicio { get; set; }

    public DateOnly? fechaFin { get; set; }

    public sbyte? permanete { get; set; }

    public sbyte? sinConvenio { get; set; }

    [Precision(5, 2)]
    public decimal? porcentaje { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idCliente")]
    [InverseProperty("convenios")]
    public virtual clientesfacturas? idClienteNavigation { get; set; }
}
