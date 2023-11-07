using CTT_Administrador.Auth.Estudiante;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CTT_Administrador.Controllers.Estudiantes
{
    [AuthorizeEstudiante]
    public class CalificacionesController : Controller
    {
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeEstudianteTools _auth;

        public CalificacionesController(IDbConnection db, IAuthorizeEstudianteTools auth)
        {
            _dapper = db;
            _auth = auth;
        }

        [HttpGet]
        public async Task<IActionResult> listarCursos()
        {
            try
            {
                string sql = @"SELECT m.idMatricula,cu.curso,t.tipoCurso,mo.modalidad,
                                YEAR(g.fechaFinCurso) AS 'year',g.fechaInicioCurso,g.fechaFinCurso,
                                t.esDiplomado
                                FROM calificaciones c
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula
                                INNER JOIN gruposcursos g ON g.idGrupoCurso = c.idGrupoCurso
                                INNER JOIN cursos cu ON cu.idCurso = g.idCurso
                                INNER JOIN modalidades mo ON g.idModalidad = mo.idModalidad
                                INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso
                                WHERE m.legalizado = 1
                                AND m.idEstudiante = @idEstudiante
                                ORDER BY idMatricula DESC;";
                return Ok(await _dapper.QueryAsync(sql, new { idEstudiante = _auth.getUser() }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> listarCalificaciones(int idMatricula)
        {
            try
            {
                string sql = @"SELECT cu.idCurso,nota1,nota2,nota3,nota4,nota5,
                                promedioFinal,faltas,justificaFaltas,
                                cu.numeroNotas,aprobado,
                                cu.curso AS modulo,
                                concat(REPLACE(i.abreviaturaTitulo,'.',''),'. ',i.primerApellido,' ',CASE WHEN LENGTH(i.segundoApellido)>0 THEN concat(i.segundoApellido,' ') ELSE ''END,
                                i.primerNombre,CASE WHEN LENGTH(i.segundoNombre)>0 THEN concat(' ',i.segundoNombre) ELSE ''END) AS instructor
                                FROM calificaciones c
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula 
                                INNER JOIN gruposcursos g ON g.idGrupoCurso = c.idGrupoCurso 
                                INNER JOIN cursos cu ON cu.idCurso = c.idCurso 
                                INNER JOIN asignacionesinstructorescalificaciones a ON a.idGrupoCurso = g.idGrupoCurso AND m.paralelo = a.paralelo AND a.idCurso = cu.idCurso
                                INNER JOIN instructores i ON i.idInstructor = a.idInstructor 
                                WHERE c.idMatricula = @idMatricula";
                return Ok(await _dapper.QueryAsync(sql, new { idMatricula }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}