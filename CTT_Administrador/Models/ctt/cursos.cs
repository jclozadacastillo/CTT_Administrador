using System;
using System.Collections.Generic;

namespace CTT_Administrador.Models.ctt;

public partial class cursos
{
    public int idCurso { get; set; }

    public int? idCategoria { get; set; }

    public int? idTipoCurso { get; set; }

    public string curso { get; set; } = null!;

    public string? imagen { get; set; }

    public decimal? precioCurso { get; set; }

    public string? objetivoPrincipal { get; set; }

    public string? objetivoSecuncdario { get; set; }

    public sbyte? tienePrecedencia { get; set; }

    public int? idCursoPrecedencia { get; set; }

    public sbyte? activo { get; set; }

    public sbyte? esVisible { get; set; }

    public int? horasCurso { get; set; }

    public sbyte? procesoUniandes { get; set; }

    public int? puntajeMaximo { get; set; }

    public int? puntajeMinimo { get; set; }

    public sbyte? calificaAsistencia { get; set; }

    public int? asistenciaMinima { get; set; }

    public int? numeroNotas { get; set; }

    public virtual ICollection<cursos_mallas> cursos_mallasidCursoAsociadoNavigation { get; set; } = new List<cursos_mallas>();

    public virtual ICollection<cursos_mallas> cursos_mallasidCursoNavigation { get; set; } = new List<cursos_mallas>();

    public virtual categorias? idCategoriaNavigation { get; set; }

    public virtual tiposcursos? idTipoCursoNavigation { get; set; }

    public virtual ICollection<temas> temas { get; set; } = new List<temas>();
}
