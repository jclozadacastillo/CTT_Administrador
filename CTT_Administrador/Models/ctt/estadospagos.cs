using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class estadospagos
{
    [Key]
    public int idEstadoPago { get; set; }

    [StringLength(50)]
    public string? estado { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idEstadoNavigation")]
    public virtual ICollection<pagosinhouse> pagosinhouse { get; set; } = new List<pagosinhouse>();
}
