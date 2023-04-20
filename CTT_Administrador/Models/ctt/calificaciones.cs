namespace CTT_Administrador.Models.ctt;

public partial class calificaciones
{
    public int idMatricula { get; set; }

    public int idGrupoCurso { get; set; }

    public int idCurso { get; set; }

    public decimal? nota1 { get; set; }

    public decimal? nota2 { get; set; }

    public decimal? nota3 { get; set; }

    public decimal? promedioFinal { get; set; }

    public decimal? faltas { get; set; }

    public sbyte? pierdeFaltas { get; set; }

    public sbyte? aprobado { get; set; }

    public sbyte? esExcento { get; set; }

    public string? observacion { get; set; }

    public decimal? nota4 { get; set; }

    public decimal? nota5 { get; set; }

    public string? justificacionObservacion { get; set; }

    public sbyte? justificaFaltas { get; set; }
}