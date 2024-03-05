using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

public partial class gruposinhouse
{
    [Key]
    public int idGrupoInHouse { get; set; }

    public int? idCliente { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaRegistro { get; set; }

    public int? idTipoDescuento { get; set; }

    [Precision(12)]
    public decimal? valorSinDescuento { get; set; }

    [Precision(10)]
    public decimal? porcentaje { get; set; }

    [StringLength(20)]
    public string? usuarioRegistro { get; set; }

    public int? idGrupoCurso { get; set; }

    [InverseProperty("idGrupoInHouseNavigation")]
    public virtual ICollection<gruposinhousemodulos> gruposinhousemodulos { get; set; } = new List<gruposinhousemodulos>();

    [InverseProperty("idGrupoInHouseNavigation")]
    public virtual ICollection<pagosinhouse> pagosinhouse { get; set; } = new List<pagosinhouse>();
}
