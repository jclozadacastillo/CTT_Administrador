using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class estudiantes
{
    public int idEstudiante { get; set; }

    public string? idTipoDocumento { get; set; }

    public string? documentoIdentidad { get; set; }

    public string? primerApellido { get; set; }

    public string? segundoApellido { get; set; }

    public string? primerNombre { get; set; }

    public string? segundoNombre { get; set; }

    public string? direccion { get; set; }

    public string? celular { get; set; }

    public string? email { get; set; }

    public string? observacion { get; set; }

    public string? sexo { get; set; }

    public sbyte? activo { get; set; }
}
