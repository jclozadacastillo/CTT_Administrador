using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class categorias
{
    [Key]
    public int idCategoria { get; set; }

    [StringLength(100)]
    public string categoria { get; set; } = null!;

    [StringLength(100)]
    public string? imagen { get; set; }

    public sbyte? activo { get; set; }

    [InverseProperty("idCategoriaNavigation")]
    public virtual ICollection<cursos> cursos { get; set; } = new List<cursos>();
}
