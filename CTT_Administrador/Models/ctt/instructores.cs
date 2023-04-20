namespace CTT_Administrador.Models.ctt;

public partial class instructores
{
    public int idInstructor { get; set; }

    public string? tipoDocumento { get; set; }

    public string? documentoIdentidad { get; set; }

    public string? primerApellido { get; set; }

    public string? segundoApellido { get; set; }

    public string? primerNombre { get; set; }

    public string? segundoNombre { get; set; }

    public string? direccion { get; set; }

    public string? telefono { get; set; }

    public string? celular { get; set; }

    public string? email { get; set; }

    public string? sexo { get; set; }

    public string? elPassword { get; set; }

    public string? abreviaturaTitulo { get; set; }

    public string? historialTitulo { get; set; }

    public string? referencia { get; set; }

    public string? historialLaboral { get; set; }

    public string? observacion { get; set; }

    public sbyte? sinDefinir { get; set; }

    public sbyte? activo { get; set; }
}