using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Controllers.Administrador
{
    public class CreditosPendientesController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;
        private readonly string _cn;

        public CreditosPendientesController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
        {
            _context = context;
            _cn = context.Database.GetConnectionString();
            _auth = auth;
            _path = _env.WebRootPath;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> listar()
        {
            var dapper = new MySqlConnection(_cn);
            try
            {
                string sql = @"
                                SELECT idEstudiante,documentoIdentidad,primerNombre,segundoNombre,primerApellido,segundoApellido,
                                sum(valorPendiente) AS deudaActual,sum(valor) AS deudaInicial
                                FROM (
                                SELECT e.idEstudiante,e.documentoIdentidad,e.primerNombre,e.segundoNombre,e.primerApellido,e.segundoApellido,
                                d.valorPendiente,d.valor
                                FROM creditos c
                                INNER JOIN detallecreditos d ON d.idCredito =c.idCredito 
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula 
                                INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante  
                                WHERE (d.cancelado IS NULL OR d.cancelado = 0) AND d.valorPendiente > 0 AND c.activo =1 AND d.activo = 1
                                ORDER BY d.fechaRegistro desc
                                ) t1
                                GROUP BY idEstudiante,documentoIdentidad,primerNombre,segundoNombre,primerApellido,segundoApellido
                              ";
                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }        
        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> detalleCreditos(int idEstudiante)
        {
            var dapper = new MySqlConnection(_cn);
            try
            {
                string sql = @"
                                SELECT c.idCredito,p.detalle,cu.curso,t.tipoCurso,sum(d.valorPendiente) AS deudaActual
                                FROM creditos c
                                INNER JOIN detallecreditos d ON d.idCredito =c.idCredito 
                                INNER JOIN matriculas m ON m.idMatricula = c.idMatricula 
                                INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante  
                                INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso 
                                INNER JOIN cursos cu ON cu.idCurso= g.idCurso 
                                INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso 
                                INNER JOIN periodos p ON p.idPeriodo = g.idPeriodo 
                                WHERE (d.cancelado IS NULL OR d.cancelado = 0) AND d.valorPendiente > 0 AND c.activo =1 AND d.activo = 1
                                AND m.idEstudiante =@idEstudiante
                                GROUP BY c.idCredito,p.detalle,cu.curso,t.tipoCurso
                                ORDER BY idCredito desc
                              ";
                var creditos = await dapper.QueryAsync(sql, new { idEstudiante });
                sql = @"SELECT idEstudiante,documentoIdentidad,primerNombre,segundoNombre,primerApellido,segundoApellido
                       FROM estudiantes WHERE idEstudiante=@idEstudiante";
                var estudiante = await dapper.QueryFirstOrDefaultAsync(sql, new { idEstudiante });
                sql = @"
                        SELECT d.idDetalleCredito,d.idCredito,date_format(d.fechaRegistro,'%Y-%m-%d') AS fechaRegistro,
                        p.detalle,cu.curso,t.tipoCurso,d.valorPendiente,d.valor
                        FROM creditos c
                        INNER JOIN detallecreditos d ON d.idCredito =c.idCredito 
                        INNER JOIN matriculas m ON m.idMatricula = c.idMatricula 
                        INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante  
                        INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso 
                        INNER JOIN cursos cu ON cu.idCurso= d.idCurso 
                        INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso 
                        INNER JOIN periodos p ON p.idPeriodo = g.idPeriodo 
                        WHERE (d.cancelado IS NULL OR d.cancelado = 0) AND d.valorPendiente > 0 AND c.activo =1 AND d.activo = 1
                        AND m.idEstudiante =@idEstudiante
                        ORDER BY idDetalleCredito desc
                        ";
                var detalleCreditos = await dapper.QueryAsync(sql, new { idEstudiante });
                return Ok(new {creditos,estudiante, detalleCreditos});
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper.Dispose();
            }
        }
    }
}