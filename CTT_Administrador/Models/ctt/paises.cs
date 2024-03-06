using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class paises
{
    [Key]
    public int idPais { get; set; }

    [StringLength(50)]
    public string? pais { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idPaisNavigation")]
    public virtual ICollection<provincias> provincias { get; set; } = new List<provincias>();
}
