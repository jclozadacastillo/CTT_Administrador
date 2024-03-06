using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idProvincia", Name = "idProvincia")]
public partial class ciudades
{
    [Key]
    public int idCiudad { get; set; }

    [StringLength(100)]
    public string? ciudad { get; set; }

    public int? idProvincia { get; set; }

    public sbyte? activo { get; set; }

    [ForeignKey("idProvincia")]
    [InverseProperty("ciudades")]
    public virtual provincias? idProvinciaNavigation { get; set; }
}
