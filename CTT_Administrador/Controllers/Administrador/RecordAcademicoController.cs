using CTT_Administrador.Auth.Administrador;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class RecordAcademicoController : Controller
    {
        private readonly IDbConnection _dapper;

        public RecordAcademicoController(IDbConnection db)
        {
            _dapper = db;
        }

        [HttpGet]
        public async Task<IActionResult> comboEstudiantes()
        {
            try
            {
                string sql = @"SELECT DISTINCT(e.idEstudiante),e.documentoIdentidad,concat(e.primerApellido,' ',CASE WHEN e.segundoApellido IS NULL THEN '' ELSE e.segundoApellido end,' ',
                                e.primerNombre,' ',CASE WHEN e.segundoNombre  IS NULL THEN '' ELSE e.segundoNombre end) AS estudiante
                                FROM calificaciones c
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula
                                INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                                INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                                INNER JOIN cursos cu ON g.idCurso = cu.idCurso
                                INNER JOIN cursos mo ON c.idCurso = mo.idCurso
                                INNER JOIN periodos p ON p.idPeriodo = g.idPeriodo
                                INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso
                                ORDER BY estudiante
                                ";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> listar(int idEstudiante)
        {
            try
            {
                string sql = @"SELECT e.documentoIdentidad,concat(e.primerApellido,' ',e.segundoApellido,' ',e.primerNombre,' ',e.segundoNombre) AS estudiante,
                                m.fechaRegistro AS fechaMatricula,t.tipoCurso,mo.curso,m.paralelo,
                                promedioFinal,CASE WHEN aprobado = 1 OR justificaFaltas =1 THEN 'APROBADO' ELSE 'REPROBADO' END AS estado,
                                faltas,CASE WHEN justificaFaltas = 1 THEN 'JUSTIFICADO' ELSE '' END AS justificado,
                                justificacionObservacion
                                FROM calificaciones c
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula
                                INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                                INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                                INNER JOIN cursos cu ON g.idCurso = cu.idCurso
                                INNER JOIN cursos mo ON c.idCurso = mo.idCurso
                                INNER JOIN periodos p ON p.idPeriodo = g.idPeriodo
                                INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso
                                WHERE e.idEstudiante=@idEstudiante
                                ORDER BY m.fechaRegistro DESC
                                ";
                return Ok(await _dapper.QueryAsync(sql, new { idEstudiante }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}