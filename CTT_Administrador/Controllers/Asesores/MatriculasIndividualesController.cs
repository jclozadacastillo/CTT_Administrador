using CTT_Administrador.Auth;
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
    public class MatriculasIndividualesController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeAsesorTools _auth;

        public MatriculasIndividualesController(cttContext context, IAuthorizeAsesorTools auth)
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

        public async Task<IActionResult> listarEstudiantes([FromQuery] string search)
        {
            try
            {
                search = search.Replace("[", "[[]").Replace("%", "[%]");
                string busqueda = $"%{search}%";
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

        [HttpPost]
        public async Task<IActionResult> cargarModulos(int idGrupoCurso, int idEstudiante)
        {
            try
            {
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
                return Ok(await _dapper.QueryFirstOrDefaultAsync(sql, new { documento }));
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
                var estudiante = await _context.estudiantes.Where(x => x.idEstudiante == _data.idEstudiante).FirstOrDefaultAsync();
                string sql = @"SELECT sum(deuda) FROM matriculas
                               WHERE idEstudiante =@idEstudiante";
                if (await _dapper.ExecuteScalarAsync<decimal>(sql, estudiante) > 0) throw new Exception("No se puede realizar una matricula de un estudiante que posee deudas pendientes.");
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
                _data.usuarioRegistro = _auth.getUser();
                _data.idCliente = clienteExistente?.idCliente > 0 ? clienteExistente.idCliente : _cliente.idCliente;
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
                sql = @"UPDATE matriculas SET deuda=(case when deuda is null then 0 else deuda end)+@deuda WHERE idMatricula=@idMatricula";
                await _dapper.ExecuteAsync(sql, _data);

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
                    var path = $"Archivos/Pagos/Asesor{_auth.getUser()}/Comprobantes/{estudiante.documentoIdentidad}/Matricula_{_data.idMatricula}/";
                    var fullPath = $"{Tools.rootPath}/{path}";
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

        [HttpGet("{idMatricula}")]
        public async Task<IActionResult> detalleMatricula(int idMatricula)
        {
            try
            {
                string sql = @"SELECT m.idMatricula,date_format(m.fechaRegistro,'%d-%m-%Y') AS fechaRegistro,
                            e.documentoIdentidad,REPLACE(concat(e.primerApellido,' ',
                            CASE WHEN e.segundoApellido IS NULL THEN '' ELSE e.segundoApellido END,' ',
                            e.primerNombre,' ',
                            CASE WHEN e.segundoNombre  IS NULL THEN '' ELSE e.segundoNombre  END
                            ),'  ',' ')AS estudiante,
                            curso,legalizado,deuda,paralelo,t.esDiplomado,
                            t.tipoCurso
                            FROM matriculas m
                            INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                            INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                            INNER JOIN cursos c ON c.idCurso = g.idCurso
                            INNER JOIN tiposcursos t ON t.idTipoCurso = c.idTipoCurso
                            WHERE idMatricula = @idMatricula";
                var matricula = await _dapper.QueryFirstOrDefaultAsync(sql, new { idMatricula });
                sql = @"SELECT cu.idCurso,cu.curso,c.fechaRegistro
                        FROM calificaciones c
                        INNER JOIN cursos cu ON cu.idCurso = c.idCurso
                        WHERE idMatricula = @idMatricula
                        ";
                var modulos = await _dapper.QueryAsync(sql, new { idMatricula });
                sql = @"SELECT idPagoMatricula,fechaPago,
                        b.banco,c.numero,t.tipo,valor,
                        p.imagenComprobante,p.numeroComprobante,fr.formaPago,tarjetaAutorizacion,tarjetaMarca,
                        e.estado,p.idEstado,p.observaciones
                        FROM pagosmatriculas p
                        INNER JOIN formaspagos f ON f.idFormaPago = p.idFormaPago
                        LEFT JOIN cuentasbancos c ON c.idCuenta = p.idCuenta
                        LEFT JOIN bancos b ON b.idBanco =c.idBanco
                        LEFT JOIN tiposcuentasbancos t ON t.idTipoCuentaBanco = c.idTipoCuentaBanco
                        INNER JOIN formaspagos fr ON fr.idFormaPago = p.idFormaPago
                        INNER JOIN estadospagos e ON e.idEstadoPago=p.idEstado
                        WHERE idMatricula = @idMatricula
                        ORDER BY p.idPagoMatricula desc";
                var pagos = await _dapper.QueryAsync(sql, new { idMatricula });
                return Ok(new { matricula, modulos, pagos });
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }
    }
}