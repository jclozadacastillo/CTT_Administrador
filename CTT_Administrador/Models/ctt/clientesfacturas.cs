using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("documento", Name = "documento", IsUnique = true)]
public partial class clientesfacturas
{
    [Key]
    public int idCliente { get; set; }

    [StringLength(1)]
    public string? idTipoDocumento { get; set; }

    [StringLength(13)]
    public string? documento { get; set; }

    [StringLength(100)]
    public string? nombre { get; set; }

    [StringLength(100)]
    public string? direccion { get; set; }

    [StringLength(10)]
    public string? telefono { get; set; }

    [StringLength(100)]
    public string? email { get; set; }

    [InverseProperty("idClienteNavigation")]
    public virtual ICollection<convenios> convenios { get; set; } = new List<convenios>();

    [InverseProperty("idClienteNavigation")]
    public virtual ICollection<pagosmatriculas> pagosmatriculas { get; set; } = new List<pagosmatriculas>();
}
