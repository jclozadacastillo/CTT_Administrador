using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Keyless]
public partial class tiposdocumentos
{
    [StringLength(1)]
    public string? idTipoDocumento { get; set; }

    [StringLength(20)]
    public string? tipo { get; set; }

    public sbyte? esCedula { get; set; }
}
