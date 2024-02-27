using CTT_Administrador.Auth.Asesor;
using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Asesores
{
    [AuthorizeAsesor]
    public class MatriculasIndividualesController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;

        public MatriculasIndividualesController(cttContext context)
        {
            _context = context;
            _dapper = _context.Database.GetDbConnection();
        }

        public async Task<IActionResult> listarPeriodos()
        {
            try
            {
                string sql = @"SELECT * FROM periodos
                                WHERE cast(current_timestamp() AS date)
                                BETWEEN fechaInicio AND fechaFin
                                AND activo = 1";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }

        public async Task<IActionResult> listarTiposCursos()
        {
            try
            {
                string sql = @"SELECT * FROM tiposcursos
                               WHERE activo =1";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> listar([FromBody] Tools.DataTableModel _params)
        {
            try
            {
                string sql = @"SELECT idMatricula,date_format(m.fechaRegistro,'%d-%m-%Y') AS fechaRegistro,
                            e.documentoIdentidad,REPLACE(concat(e.primerApellido,' ',
                            CASE WHEN e.segundoApellido IS NULL THEN '' ELSE e.segundoApellido END,' ',
                            e.primerNombre,' ',
                            CASE WHEN e.segundoNombre  IS NULL THEN '' ELSE e.segundoNombre  END
                            ),'  ',' ')AS estudiante,
                            curso,legalizado,deuda,paralelo
                            FROM matriculas m
                            INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                            INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                            INNER JOIN cursos c ON c.idCurso = g.idCurso
                            WHERE g.idPeriodo=@idPeriodo AND c.idTipoCurso=@idTipoCurso
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

        [HttpPost]
        public async Task<IActionResult> listarCursos(int idPeriodo, int idTipoCurso)
        {
            try
            {
                string sql = @"SELECT idGrupoCurso,c.curso
                                FROM gruposcursos g
                                INNER JOIN cursos c ON c.idCurso = g.idCurso
                                WHERE cast(current_timestamp() AS date)
                                BETWEEN g.fechaInicioMatricula AND g.fechaFinMatricula
                                AND idPeriodo=@idPeriodo AND idTipoCurso =@idTipoCurso";
                return Ok(await _dapper.QueryAsync(sql, new { idPeriodo, idTipoCurso }));
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }

        public async Task<IActionResult> listarEstudiantes([FromQuery]string search)
        {
            try
            {
                search = search.Replace("[", "[[]").Replace("%", "[%]");
                string  busqueda= $"%{search}%";
                string sql = @"SELECT idEstudiante as id,
                               replace(concat(documentoIdentidad,' - ',
                               primerApellido,' ',
                               CASE WHEN segundoApellido IS NULL THEN '' ELSE segundoApellido END,' ',
                               primerNombre,' ',
                               CASE WHEN segundoNombre IS NULL THEN '' ELSE segundoNombre END),'  ',' ') as text
                               FROM Estudiantes
                               WHERE replace(concat(documentoIdentidad,' - ',
                               primerApellido,' ',
                               CASE WHEN segundoApellido IS NULL THEN '' ELSE segundoApellido END,' ',
                               primerNombre,' ',
                               CASE WHEN segundoNombre IS NULL THEN '' ELSE segundoNombre END),'  ',' ') like @busqueda;
                               ";
                return Ok(await _dapper.QueryAsync(sql, new { busqueda }));
            }
            catch (Exception ex)
            {

                return Tools.handleError(ex);
            }
        }
    }
}