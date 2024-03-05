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
    public class RecaudacionInHouseController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeAsesorTools _auth;

        public RecaudacionInHouseController(cttContext context, IAuthorizeAsesorTools auth)
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
                string sql = @"SELECT idGrupoInHouse,date_format(m.fechaRegistro,'%d-%m-%Y') AS fechaRegistro,
                            e.documento,nombre cliente,
                            curso,valorSinDescuento,t.nombreDescuento,m.porcentaje,
                            valorSinDescuento-((valorSinDescuento*m.porcentaje)/100)-
                            CASE WHEN (SELECT sum(valor) FROM pagosinhouse WHERE idGrupoInHouse=m.idGrupoInHouse)IS NULL THEN 0
                            ELSE (SELECT sum(valor) FROM pagosinhouse WHERE idGrupoInHouse=m.idGrupoInHouse) END
                            as deuda
                            FROM gruposinhouse m
                            INNER JOIN gruposcursos g ON g.idGrupoCurso = m.idGrupoCurso
                            INNER JOIN clientesfacturas e ON e.idCliente= m.idCliente
                            INNER JOIN cursos c ON c.idCurso = g.idCurso
                            INNER JOIN tiposDescuentos t on t.idTipoDescuento = m.idTipoDescuento
                            WHERE usuarioRegistro=@usuario
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
        public async Task<IActionResult> agregarPago(pagosinhouse _pago, IFormFile archivoComprobante)
        {
            try
            {
                string sql = @"SELECT * FROM gruposinhouse WHERE idGrupoInHouse=@idGrupoInHouse";
                var _data = await _dapper.QueryFirstOrDefaultAsync<gruposinhouse>(sql, _pago);
                sql = @"SELECT * FROM clientesfacturas WHERE idCliente=@idCliente";
                var cliente = await _dapper.QueryFirstOrDefaultAsync<clientesfacturas>(sql, _data);
                _pago.idGrupoInHouse = _data.idGrupoInHouse;
                _pago.idEstado = 1;
                _context.pagosinhouse.Add(_pago);
                await _context.SaveChangesAsync();
                if (archivoComprobante != null)
                {
                    var path = $"Archivos/Pagos/Asesor{_auth.getUser()}/Comprobantes/{cliente.documento}/MatriculaInHouse_{_data.idGrupoInHouse}/";
                    var fullPath = $"{Tools.rootPath}/{path}";
                    if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                    using (FileStream fs = System.IO.File.Create($"{fullPath}{_pago.idPago}.{archivoComprobante.FileName.Split(".").Last()}")) archivoComprobante.CopyTo(fs);
                    _pago.imagenComprobante = $"{path}{_pago.idPago}.{archivoComprobante.FileName.Split(".").Last()}";
                    _context.pagosinhouse.Update(_pago);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{idGrupoInHouse}")]
        public async Task<IActionResult> detalleMatriculas(int idGrupoInHouse)
        {
            try
            {
                string sql = @"SELECT m.idMatricula,e.documentoIdentidad,
                            LTRIM(REPLACE(concat(e.primerApellido,' ',
                            CASE WHEN e.segundoApellido IS NULL THEN '' ELSE e.segundoApellido END,' ',
                            e.primerNombre,' ',
                            CASE WHEN e.segundoNombre  IS NULL THEN '' ELSE e.segundoNombre  END
                            ),'  ',' '))AS estudiante
                            FROM matriculas m
                            INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                            WHERE idGrupoInHouse=@idGrupoInHouse;
                            ";
                var alumnos = await _dapper.QueryAsync(sql, new { idGrupoInHouse });
                sql = @"SELECT curso  FROM gruposinhousemodulos gm
                        INNER JOIN cursos c ON c.idCurso = gm.idCurso
                        WHERE idGrupoInHouse = @idGrupoInHouse";
                var modulos = await _dapper.QueryAsync(sql, new { idGrupoInHouse });
                sql = @"SELECT gi.idGrupoInHouse,c.documento,c.nombre,
                        cu.curso,t.tipoCurso,gi.porcentaje,gi.valorSinDescuento,esDiplomado,
                        (select sum(valor) FROM pagosinhouse WHERE idGrupoInHouse=gi.idGrupoInHouse) as valorPagado
                        FROM gruposinhouse gi
                        INNER JOIN gruposcursos g ON g.idGrupoCurso = gi.idGrupoCurso
                        INNER JOIN clientesfacturas c ON c.idCliente = gi.idCliente
                        INNER JOIN cursos cu ON cu.idCurso = g.idCurso
                        INNER JOIN tiposcursos t ON t.idTipoCurso = cu.idTipoCurso
                        WHERE gi.idGrupoInHouse = @idGrupoInHouse";
                var info = await _dapper.QueryFirstOrDefaultAsync(sql, new { idGrupoInHouse });
                sql = @"SELECT idPago,fechaPago,
                        b.banco,c.numero,t.tipo,valor,
                        p.imagenComprobante,p.numeroComprobante,fr.formaPago,tarjetaAutorizacion,tarjetaMarca
                        FROM pagosinhouse p
                        INNER JOIN formaspagos f ON f.idFormaPago = p.idFormaPago
                        LEFT JOIN cuentasbancos c ON c.idCuenta = p.idCuenta
                        LEFT JOIN bancos b ON b.idBanco =c.idBanco
                        LEFT JOIN tiposcuentasbancos t ON t.idTipoCuentaBanco = c.idTipoCuentaBanco
                        INNER JOIN formaspagos fr ON fr.idFormaPago = p.idFormaPago
                        WHERE idGrupoInHouse = @idGrupoInHouse
                        ORDER BY p.idPago desc";
                var pagos = await _dapper.QueryAsync(sql, new { idGrupoInHouse });
                return Ok(new { alumnos, modulos, info, pagos });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}