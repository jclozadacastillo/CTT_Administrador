using CTT_Administrador.Auth.Contador;
using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Contadores
{
    [AuthorizeContador]
    [Route("Contadores/{controller}/{action}")]
    public class ValidarPagosController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeContadorTools _auth;

        public ValidarPagosController(cttContext context, IAuthorizeContadorTools auth)
        {
            _context = context;
            _dapper = _context.Database.GetDbConnection();
            _auth = auth;
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
                            curso,legalizado,deuda,paralelo,
                            (SELECT sum(valor) FROM pagosmatriculas WHERE idMatricula=m.idMatricula AND idEstado=0) as pendienteValidacion
                            FROM matriculas m
                            INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                            INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                            INNER JOIN cursos c ON c.idCurso = g.idCurso
                            WHERE inHouse=0 and (SELECT sum(valor) FROM pagosmatriculas WHERE idMatricula=m.idMatricula AND idEstado=0)>0
                                ";
                return Ok(await Tools.DataTableMySql(new Tools.DataTableParams
                {
                    query = sql,
                    dapperConnection = _context.Database.GetDbConnection(),
                    dataTableModel = _params,
                    parameters = new { usuario = _auth.getUser() }
                }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> listarValidados([FromBody] Tools.DataTableModel _params)
        {
            try
            {
                string sql = @"SELECT idMatricula,date_format(m.fechaRegistro,'%d-%m-%Y') AS fechaRegistro,
                            e.documentoIdentidad,REPLACE(concat(e.primerApellido,' ',
                            CASE WHEN e.segundoApellido IS NULL THEN '' ELSE e.segundoApellido END,' ',
                            e.primerNombre,' ',
                            CASE WHEN e.segundoNombre  IS NULL THEN '' ELSE e.segundoNombre  END
                            ),'  ',' ')AS estudiante,
                            curso,legalizado,deuda,paralelo,
                            (SELECT sum(valor) FROM pagosmatriculas WHERE idMatricula=m.idMatricula AND idEstado=0) as pendienteValidacion
                            FROM matriculas m
                            INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                            INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                            INNER JOIN cursos c ON c.idCurso = g.idCurso
                            WHERE inHouse=0 and (SELECT count(*) FROM pagosmatriculas WHERE idMatricula=m.idMatricula AND idEstado=0)=0
                            AND m.idMatricula in(SELECT idMatricula FROM pagosmatriculas WHERE idMatricula=m.idMatricula)
                                ";
                return Ok(await Tools.DataTableMySql(new Tools.DataTableParams
                {
                    query = sql,
                    dapperConnection = _context.Database.GetDbConnection(),
                    dataTableModel = _params,
                    parameters = new { usuario = _auth.getUser() }
                }));
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
        public async Task<IActionResult> agregarPago(pagosmatriculas _pago, IFormFile archivoComprobante)
        {
            try
            {
                string sql = @"SELECT * FROM Matriculas WHERE idMatricula=@idMatricula";
                var _data = await _dapper.QueryFirstOrDefaultAsync<matriculas>(sql, _pago);
                sql = @"SELECT * FROM estudiantes WHERE idEstudiante=@idEstudiante";
                var estudiante = await _dapper.QueryFirstOrDefaultAsync<estudiantes>(sql, _data);
                _pago.idMatricula = _data.idMatricula;
                _pago.idEstado = 1;
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
                sql = @"UPDATE Matriculas set deuda=deuda-@valor WHERE idMatricula=@idMatricula";
                await _dapper.ExecuteAsync(sql, _pago);
                sql = @"SELECT sum(deuda) FROM Matriculas WHERE idMatricula=@idMatricula";
                sql = await _dapper.ExecuteScalarAsync<decimal>(sql, _data) == 0 ?
                @"UPDATE Matriculas SET legalizado=1 WHERE idMatricula=@idMatricula" :
                @"UPDATE Matriculas SET legalizado=0 WHERE idMatricula=@idMatricula";
                await _dapper.ExecuteAsync(sql, _data);
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
                            t.tipoCurso,
                            (SELECT sum(valor) FROM pagosmatriculas WHERE idMatricula=m.idMatricula AND idEstado=0) as pendienteValidacion
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
                        e.estado,e.idEstadoPago as idEstado,p.observaciones
                        FROM pagosmatriculas p
                        INNER JOIN formaspagos f ON f.idFormaPago = p.idFormaPago
                        INNER JOIN estadospagos e on e.idEstadoPago=p.idEstado
                        LEFT JOIN cuentasbancos c ON c.idCuenta = p.idCuenta
                        LEFT JOIN bancos b ON b.idBanco =c.idBanco
                        LEFT JOIN tiposcuentasbancos t ON t.idTipoCuentaBanco = c.idTipoCuentaBanco
                        INNER JOIN formaspagos fr ON fr.idFormaPago = p.idFormaPago
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

        [HttpPost]
        public async Task<IActionResult> validarPago(pagosmatriculas _data)
        {
            try
            {
                _data.usuarioValidacion = _auth.getUser();
                _data.fechaValidacion = DateTime.Now;
                var sql = @"UPDATE pagosmatriculas
                            SET idEstado=@idEstado,
                            usuarioValidacion=@usuarioValidacion,
                            fechaValidacion=@fechaValidacion,
                            observaciones=@observaciones
                            WHERE idPagoMatricula=@idPagoMatricula";
                await _dapper.ExecuteAsync(sql, _data);
                if (_data.idEstado == -1)
                {
                    sql = @"SELECT * FROM pagosmatriculas WHERE idPagoMatricula=@idPagoMatricula";
                    var pago = await _dapper.QueryFirstOrDefaultAsync<pagosmatriculas>(sql, _data);
                    sql = @"UPDATE matriculas set deuda=deuda+@valor
                            WHERE idMatricula=@idMatricula";
                    await _dapper.ExecuteAsync(sql, pago);
                }
                sql = @"SELECT sum(deuda) FROM Matriculas WHERE idMatricula=@idMatricula";
                sql = await _dapper.ExecuteScalarAsync<decimal>(sql, _data) == 0 ?
                @"UPDATE Matriculas SET legalizado=1 WHERE idMatricula=@idMatricula" :
                @"UPDATE Matriculas SET legalizado=0 WHERE idMatricula=@idMatricula";
                await _dapper.ExecuteAsync(sql, _data);
                return Ok();
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }
    }
}