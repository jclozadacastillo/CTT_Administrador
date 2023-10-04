using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class formaspagos
{
    [Key]
    public int idFormaPago { get; set; }

    [StringLength(60)]
    public string? formaPago { get; set; }

    [StringLength(20)]
    public string? codigo_financiero { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idFormaPagoNavigation")]
    public virtual ICollection<pagosmatriculas> pagosmatriculas { get; set; } = new List<pagosmatriculas>();
}
