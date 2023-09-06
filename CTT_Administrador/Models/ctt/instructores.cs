using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("documentoIdentidad", Name = "documentoIdentidad", IsUnique = true)]
public partial class instructores
{
    [Key]
    public int idInstructor { get; set; }

    [StringLength(1)]
    public string? tipoDocumento { get; set; }

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
    public string? telefono { get; set; }

    [StringLength(10)]
    public string? celular { get; set; }

    [StringLength(100)]
    public string? email { get; set; }

    [StringLength(10)]
    public string? sexo { get; set; }

    [StringLength(60)]
    public string? elPassword { get; set; }

    [StringLength(10)]
    public string? abreviaturaTitulo { get; set; }

    [StringLength(1000)]
    public string? historialTitulo { get; set; }

    [StringLength(1000)]
    public string? referencia { get; set; }

    [StringLength(1000)]
    public string? historialLaboral { get; set; }

    [StringLength(400)]
    public string? observacion { get; set; }

    public sbyte? sinDefinir { get; set; }

    public sbyte? activo { get; set; }
}
