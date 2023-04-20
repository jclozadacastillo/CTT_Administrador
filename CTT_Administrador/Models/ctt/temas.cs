namespace CTT_Administrador.Models.ctt;

public partial class temas
{
    public int idTema { get; set; }

    public int? idCurso { get; set; }

    public sbyte? esTitulo { get; set; }

    public string? tema { get; set; }

    public int? orden { get; set; }

    public sbyte? activo { get; set; }

    public virtual cursos? idCursoNavigation { get; set; }
}