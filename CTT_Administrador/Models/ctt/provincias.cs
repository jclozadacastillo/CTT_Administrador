using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idPais", Name = "idPais")]
public partial class provincias
{
    [Key]
    public int idProvincia { get; set; }

    [StringLength(100)]
    public string? provincia { get; set; }

    public int? idPais { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idProvinciaNavigation")]
    public virtual ICollection<ciudades> ciudades { get; set; } = new List<ciudades>();

    [ForeignKey("idPais")]
    [InverseProperty("provincias")]
    public virtual paises? idPaisNavigation { get; set; }
}
