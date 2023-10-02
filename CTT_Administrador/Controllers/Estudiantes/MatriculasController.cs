using CTT_Administrador.Auth.Estudiante;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CTT_Administrador.Controllers.Estudiantes
{
    [AuthorizeEstudiante]
    public class MatriculasController : Controller
    {
        private readonly IAuthorizeEstudianteTools _auth;
        private readonly IDbConnection _dapper;
        private readonly cttContext _context;

        public MatriculasController(IAuthorizeEstudianteTools auth, IDbConnection db, cttContext context)
        {
            _auth = auth;
            _dapper = db;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> comboTiposCursos()
        {
            try
            {
                string sql = @"SELECT distinct(t.idTipoCurso),t.tipoCurso 
                               FROM cursos c
                               INNER JOIN gruposcursos g ON g.idCurso = c.idCurso 
                               INNER JOIN categorias ca ON ca.idCategoria = c.idCategoria
                               INNER JOIN tiposcursos t ON t.idTipoCurso = c.idTipoCurso
                               WHERE current_date() BETWEEN g.fechaInicioMatricula AND g.fechaFinMatricula ";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> comboCategorias(int idTipoCurso)
        {
            try
            {
                string sql = @"SELECT distinct(ca.idCategoria),ca.categoria 
                            FROM cursos c
                            INNER JOIN gruposcursos g ON g.idCurso = c.idCurso 
                            INNER JOIN categorias ca ON ca.idCategoria = c.idCategoria
                            INNER JOIN tiposcursos t ON t.idTipoCurso = c.idTipoCurso
                            WHERE current_date() BETWEEN g.fechaInicioMatricula AND g.fechaFinMatricula
                            AND c.idTipoCurso = @idTipoCurso
                            ORDER BY ca.categoria ";
                return Ok(await _dapper.QueryAsync(sql, new {idTipoCurso}));
            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }
        }
                
        [HttpPost]
        public async Task<IActionResult> comboGruposCursos(int idCategoria,int idTipoCurso)
        {
            try
            {
                string sql = @"SELECT distinct(g.idGrupoCurso),c.curso
                                FROM cursos c
                                INNER JOIN gruposcursos g ON g.idCurso = c.idCurso 
                                INNER JOIN categorias ca ON ca.idCategoria = c.idCategoria
                                INNER JOIN tiposcursos t ON t.idTipoCurso = c.idTipoCurso
                                WHERE current_date() BETWEEN g.fechaInicioMatricula AND g.fechaFinMatricula
                                AND t.idTipoCurso = @idTipoCurso AND c.idCategoria = @idCategoria
                                ORDER BY curso";
                return Ok(await _dapper.QueryAsync(sql, new {idTipoCurso,idCategoria}));
            }
            catch (Exception ex)
            {

                return Problem(ex.Message);
            }
        }


    }
}
