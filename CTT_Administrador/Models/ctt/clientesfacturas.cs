using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class clientesfacturas
{
    public int idCliente { get; set; }

    public string? idTipoDocumento { get; set; }

    public string? documento { get; set; }

    public string? nombre { get; set; }

    public string? direccion { get; set; }

    public string? telefono { get; set; }

    public string? email { get; set; }
}
