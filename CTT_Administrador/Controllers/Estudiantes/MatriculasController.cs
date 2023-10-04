using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Estudiante;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return Ok(await _dapper.QueryAsync(sql, new { idTipoCurso }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> comboGruposCursos(int idCategoria, int idTipoCurso)
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
                return Ok(await _dapper.QueryAsync(sql, new { idTipoCurso, idCategoria }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> cargarModulos(int idGrupoCurso)
        {
            try
            {
                var idEstudiante = _auth.getUser();
                string sql = @"SELECT * FROM matriculas WHERE idEstudiante=@idEstudiante AND idGrupoCurso=@idGrupoCurso";
                var matricula = await _dapper.QueryFirstOrDefaultAsync(sql, new { idGrupoCurso, idEstudiante });
                int idMatricula = (matricula != null) ? matricula.idMatricula : 0;

                sql = @"SELECT distinct(c.idCurso),curso,cm.valor,numeroModulo
                                FROM cursos_mallas cm
                                INNER JOIN gruposcursos g ON g.idCurso = cm.idCurso
                                INNER JOIN cursos c ON c.idCurso = cm.idCursoAsociado
                                WHERE idGrupoCurso = @idGrupoCurso
                                AND c.idCurso NOT IN(SELECT idCurso FROM calificaciones
                                WHERE idCurso = c.idCurso AND idMatricula = @idMatricula
                                )
                                ORDER BY numeroModulo";
                return Ok(new
                {
                    listaModulos = await _dapper.QueryAsync(sql, new { idGrupoCurso, idMatricula }),
                    idMatricula
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboFormasPagos()
        {
            try
            {
                string sql = @"SELECT * FROM formasPagos
                                WHERE activo=1
                                ORDER BY formaPago";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboCuentas()
        {
            try
            {
                string sql = @"SELECT idCuenta,alias
                                FROM cuentasbancos
                                WHERE activo = 1";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboTiposDocumentos()
        {
            try
            {
                string sql = @"SELECT * FROM tiposDocumentos";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> datosCliente(string documento)
        {
            try
            {
                string sql = @"SELECT * FROM clientesfacturas WHERE documento=@documento";
                return Ok(await _dapper.QueryFirstOrDefaultAsync(sql, new {documento}));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> generarMatricula(string modulos, pagosmatriculas _pago, matriculas _data, clientesfacturas _cliente, IFormFile archivoComprobante)
        {
            try
            {
                _data.idEstudiante = Convert.ToInt32(_auth.getUser());
                var estudiante = await _context.estudiantes.Where(x => x.idEstudiante == _data.idEstudiante).FirstOrDefaultAsync();
                var clienteExistente = await _context.clientesfacturas.Where(x => x.documento == _cliente.documento).FirstOrDefaultAsync();
                if (clienteExistente == null)
                {
                    _context.clientesfacturas.Add(_cliente);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    clienteExistente.telefono = _cliente.telefono;
                    clienteExistente.direccion = _cliente.direccion;
                    clienteExistente.email = _cliente.email;
                    _context.clientesfacturas.Update(clienteExistente);
                    await _context.SaveChangesAsync();
                }
                _data.usuarioRegistro = "web";
                _data.paralelo = "1";
                _data.idCliente = clienteExistente?.idCliente > 0 ? clienteExistente.idCliente : _cliente.idCliente;
                string sql = "";
                if (_data.idMatricula == 0)
                {
                    sql = $@"
                        INSERT INTO matriculas
                        (idEstudiante, idCliente, idGrupoCurso, idTipoDescuento, paralelo,
                        fechaRegistro, esUniandes, idCarrera, idCentro, usuarioRegistro,legalizado)
                        select distinct(e.idEstudiante),
                        @idCliente,
                        @idGrupoCurso,1 as idTipoDescuento,@paralelo,
                        current_timestamp(),1,null,null,@usuarioRegistro,0
                        from estudiantes e
                        where e.idEstudiante=@idEstudiante
                        and e.idEstudiante not in(select m.idEstudiante
                        from matriculas m
                        inner join gruposcursos g on m.idGrupoCurso = @idGrupoCurso
                        );
                        SELECT LAST_INSERT_ID();
                        ";
                    _data.idMatricula = await _dapper.ExecuteScalarAsync<int>(sql, _data);
                }

                if (!(_data.idMatricula > 0)) throw new Exception("La matricula no sé generó exitosamente vuelva a intentarlo");
                sql = $@"
                        insert into calificaciones (idMatricula,idGrupoCurso,idCurso)
                        select
                        idMatricula,g.idGrupoCurso,cm.idCursoAsociado
                        from matriculas m
                        inner join gruposcursos g on g.idGrupoCurso = m.idGrupoCurso
                        inner join estudiantes e on e.idEstudiante = m.idEstudiante
                        inner join cursos c on c.idCurso=g.idCurso
                        inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                        inner join cursos_mallas cm on cm.idCurso = g.idCurso
                        where c.activo = 1
                        and cm.idCursoAsociado in(
                        {modulos}
                        )
                        and concat(cast(m.idMatricula as char),'_',
                        cast(m.idGrupoCurso as char),'_',cast(cm.idCursoAsociado as char))
                        not in(
                        select concat(cast(idMatricula as char),'_',
                        cast(idGrupoCurso as char),'_',cast(idCurso as char))
                        from calificaciones ca where ca.idMatricula=m.idMatricula
                        ) and m.idMatricula = @idMatricula
                    ";
                await _dapper.ExecuteAsync(sql, _data);

                _pago.idMatricula = _data.idMatricula;
                _pago.idEstado = 0;
                _pago.idCliente = _data.idCliente;
                _context.pagosmatriculas.Add(_pago);
                await _context.SaveChangesAsync();
                if (archivoComprobante != null)
                {
                    var path = $"Archivos/Estudiantes/{estudiante.documentoIdentidad}/Comprobantes/";
                    var fullPath = $"{ConfigurationHelper.host}/{path}";
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    using (FileStream fs = System.IO.File.Create($"{fullPath}{_pago.idPagoMatricula}.{archivoComprobante.FileName.Split(".").Last()}")) archivoComprobante.CopyTo(fs);
                    _pago.imagenComprobante = $"{path}{_pago.idPagoMatricula}.{archivoComprobante.FileName.Split(".").Last()}";
                    _context.pagosmatriculas.Update(_pago);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}