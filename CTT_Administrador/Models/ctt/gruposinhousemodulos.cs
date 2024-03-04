using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Models.ctt;

[Index("idGrupoInHouse", Name = "idGrupoInHouse")]
public partial class gruposinhousemodulos
{
    [Key]
    public int idGrupoInHouseModulo { get; set; }

    public int? idGrupoInHouse { get; set; }

    public int? idCurso { get; set; }

    [ForeignKey("idGrupoInHouse")]
    [InverseProperty("gruposinhousemodulos")]
    public virtual gruposinhouse? idGrupoInHouseNavigation { get; set; }
}
