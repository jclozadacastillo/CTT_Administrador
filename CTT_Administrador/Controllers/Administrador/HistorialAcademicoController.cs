using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    [Route("{controller}/{action}")]
    public class HistorialAcademicoController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;

        public HistorialAcademicoController(cttContext context)
        {
            _context = context;
            _dapper = _context.Database.GetDbConnection();
        }

        [HttpPost]
        public async Task<IActionResult> listar([FromBody] Tools.DataTableModel _params)
        {
            try
            {
                string sql = @"SELECT idEstudiante,documentoIdentidad,
                            REPLACE(concat(primerApellido,' ',
                            CASE WHEN segundoApellido IS NULL THEN '' ELSE segundoApellido END,' ',
                            primerNombre,' ',
                            CASE WHEN segundoNombre IS NULL THEN '' ELSE segundoNombre END),'  ',' ') as estudiante,
                            e.activo, celular,email,ciudad,provincia
                            FROM estudiantes e
                            INNER JOIN ciudades c on c.idCiudad=e.idCiudad
                            INNER JOIN provincias p on p.idProvincia=c.idProvincia
                            WHERE idEstudiante in (
                            SELECT m.idEstudiante FROM calificaciones c
                            INNER JOIN matriculas m ON m.idMatricula=c.idMatricula AND m.idEstudiante=e.idEstudiante
                            )
                                ";
                return Ok(await Tools.DataTableMySql(new Tools.DataTableParams
                {
                    query = sql,
                    dapperConnection = _context.Database.GetDbConnection(),
                    dataTableModel = _params
                }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{idEstudiante}")]
        public async Task<IActionResult> historialAcademico(int idEstudiante)
        {
            try
            {
                string sql = @"SELECT * FROM Estudiantes WHERE idEstudiante=@idEstudiante";
                var estudiante = await _dapper.QueryFirstOrDefaultAsync<estudiantes>(sql, new { idEstudiante });
                sql = @"SELECT m.idMatricula,c.curso,p.detalle,
                        t.tipoCurso,c.calificaAsistencia,c.numeroNotas,g.idGrupoCurso,t.esDiplomado
                        FROM matriculas m
                        INNER JOIN gruposcursos g ON g.idGrupoCurso =m.idGrupoCurso
                        INNER JOIN cursos c ON c.idCurso = g.idCurso
                        INNER JOIN periodos p ON p.idPeriodo = g.idPeriodo
                        INNER JOIN tiposcursos t ON t.idTipoCurso = c.idTipoCurso
                        WHERE m.idEstudiante  = @idEstudiante";
                var matriculas = await _dapper.QueryAsync(sql, new { idEstudiante });
                sql = @"SELECT c.idMatricula,
                        cr.curso,nota1,nota2,
                        nota3,nota4,nota5,promedioFinal,
                        faltas,pierdeFaltas,aprobado,
                        justificaFaltas,justificacionObservacion,
                        c.fechaRegistro
                        FROM calificaciones c
                        INNER JOIN cursos cr ON cr.idCurso = c.idCurso
                        WHERE c.idMatricula in(SELECT idMatricula
                        FROM matriculas WHERE idEstudiante = @idEstudiante
                        )";
                var calificaciones = await _dapper.QueryAsync(sql, new { idEstudiante });
                return Ok(new { estudiante, matriculas, calificaciones });
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }
    }
}