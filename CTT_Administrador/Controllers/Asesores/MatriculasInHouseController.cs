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
    [Route("Asesores/{controller}/{action}")]
    public class MatriculasInHouseController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeAsesorTools _auth;

        public MatriculasInHouseController(cttContext context, IAuthorizeAsesorTools auth)
        {
            _context = context;
            _dapper = _context.Database.GetDbConnection();
            _auth = auth;
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

        public async Task<IActionResult> listarClientes([FromQuery] string search)
        {
            try
            {
                search = search.Replace("[", "[[]").Replace("%", "[%]");
                string busqueda = $"%{search}%";
                string sql = @"SELECT idCliente as id,
                               replace(concat(documento,' - ',nombre),'  ',' ') as text
                               FROM clientesfacturas
                               WHERE replace(concat(documento,' - ',nombre),'  ',' ') like @busqueda;
                               ";
                return Ok(await _dapper.QueryAsync(sql, new { busqueda }));
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> cargarModulos(int idGrupoCurso)
        {
            try
            {
                string sql = @"SELECT distinct(c.idCurso),curso,cm.valor,numeroModulo
                                FROM cursos_mallas cm
                                INNER JOIN gruposcursos g ON g.idCurso = cm.idCurso
                                INNER JOIN cursos c ON c.idCurso = cm.idCursoAsociado
                                WHERE idGrupoCurso = @idGrupoCurso
                                ORDER BY numeroModulo";
                return Ok(new
                {
                    listaModulos = await _dapper.QueryAsync(sql, new { idGrupoCurso }),
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}