using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("documentoIdentidad", Name = "documentoIdentidad", IsUnique = true)]
public partial class estudiantes
{
    [Key]
    public int idEstudiante { get; set; }

    [StringLength(1)]
    public string? idTipoDocumento { get; set; }

    [StringLength(13)]
    public string? documentoIdentidad { get; set; }

    [StringLength(40)]
    public string? primerApellido { get; set; }

    [StringLength(40)]
    public string? segundoApellido { get; set; }

    [StringLength(40)]
    public string? primerNombre { get; set; }

    [StringLength(40)]
    public string? segundoNombre { get; set; }

    [StringLength(100)]
    public string? direccion { get; set; }

    [StringLength(10)]
    public string? celular { get; set; }

    [StringLength(100)]
    public string? email { get; set; }

    [StringLength(200)]
    public string? observacion { get; set; }

    [StringLength(10)]
    public string? sexo { get; set; }

    public sbyte? activo { get; set; }

    public sbyte? confirmado { get; set; }

    [StringLength(50)]
    public string? clave { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? fechaConfirmacion { get; set; }

    public int? idCiudad { get; set; }
}
