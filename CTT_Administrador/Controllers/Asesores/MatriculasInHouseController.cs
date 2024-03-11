using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace CTT_Administrador.Controllers.Asesores
{
    [AuthorizeAdministrador]
    [Route("Administrador/{controller}/{action}")]
    public class MatriculasInHouseController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;
        private readonly IAuthorizeAdministradorTools _auth;

        public MatriculasInHouseController(cttContext context, IAuthorizeAdministradorTools auth)
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
                string sql = @"SELECT g.idGrupoInHouse,date_format(g.fechaRegistro,'%d-%m-%Y') AS fechaRegistro,
                            c.documento,c.nombre,g.valorSinDescuento,porcentaje,
                            (SELECT count(*) FROM matriculas WHERE idGrupoInHouse=g.idGrupoInHouse) AS estudiantes,
                            cu.curso
                            FROM gruposinhouse g
                            INNER JOIN gruposcursos gc ON gc.idGrupoCurso = g.idGrupoCurso
                            INNER JOIN clientesfacturas c ON c.idCliente = g.idCliente
                            INNER JOIN cursos cu ON cu.idCurso = gc.idCurso
                            WHERE gc.idPeriodo=@idPeriodo AND cu.idTipoCurso=@idTipoCurso
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
        public async Task<IActionResult> listarCiudades()
        {
            try
            {
                return Ok(await _context.ciudades.ToListAsync());
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }
        public async Task<IActionResult> listarTiposDescuentos()
        {
            try
            {
                var lista = await (from d in _context.tiposdescuentos
                                   where d.activo == 1
                                   select new
                                   {
                                       d.idTipoDescuento,
                                       d.nombreDescuento,
                                       d.porcentaje,
                                       d.sinDescuento
                                   }).OrderBy(x => x.porcentaje).ToListAsync();
                return Ok(lista);
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

        [HttpPost]
        public async Task<IActionResult> leerExcel(IFormFile archivoParticipantes)
        {
            try
            {
                var stream = archivoParticipantes.OpenReadStream();
                return Ok(await procesarCampos(stream));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private async Task<List<dynamic>> procesarCampos(Stream stream)
        {
            try
            {
                var excel = new XSSFWorkbook(stream);
                var hoja = excel.GetSheetAt(0);
                var listaProcesada = procesarHoja(hoja);
                return listaProcesada;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
        }

        private List<dynamic> procesarHoja(ISheet hoja)
        {
            try
            {
                var cabecera = hoja.GetRow(0).ToList();
                var lista = new List<dynamic>();
                for (int i = 1; i <= hoja.LastRowNum; i++)
                {
                    var fila = hoja.GetRow(i);
                    var objeto = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
                    for (int x = 0; x < cabecera.Count; x++)
                    {
                        var key = cabecera[x].ToString().Replace(" ", "_")
                        .Replace("á", "a")
                        .Replace("é", "e")
                        .Replace("í", "i")
                        .Replace("ó", "o")
                        .Replace("ú", "u");
                        var type = fila.GetCell(x)?.CellType.ToString().ToLower();
                        if (type != null)
                        {
                            if (type.StartsWith("str"))
                            {
                                var value = fila.GetCell(x).StringCellValue;
                                objeto.Add(key, value);
                            }
                            if (type.StartsWith("nu"))
                            {
                                var value = fila.GetCell(x).NumericCellValue;
                                objeto.Add(key, value);
                            }
                            if (type.StartsWith("da"))
                            {
                                var value = fila.GetCell(x).DateCellValue;
                                objeto.Add(key, value);
                            }
                        }
                        else
                        {
                            objeto.Add(key, "");
                        }
                    }
                    lista.Add(objeto);
                }
                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public class paralelosEstudiantes
        {
            public string documentoIdentidad { get; set; }
            public string paralelo { get; set; }
        }

        public async Task<IActionResult> guardarMatriculas(matriculas _matricula, string participantes, string modulos, int idTipoDescuento, int idCliente)
        {
            IDbConnection dapperMultiple = _context.Database.GetDbConnection();
            try
            {
                var estudiantes = JsonConvert.DeserializeObject<List<estudiantes>>(participantes);
                var paralelos = JsonConvert.DeserializeObject<List<paralelosEstudiantes>>(participantes);
                var listaEstudiantesGuardar = new List<estudiantes>();
                foreach (var item in estudiantes)
                {
                    item.idEstudiante = await _context.estudiantes.AsNoTracking().Where(x => x.documentoIdentidad == item.documentoIdentidad).Select(x => x.idEstudiante).FirstOrDefaultAsync();
                    if (!(item.idEstudiante > 0))
                    {
                        if (item.documentoIdentidad?.Length == 10) item.idTipoDocumento = "C";
                        else item.idTipoDocumento = "P";
                        listaEstudiantesGuardar.Add(item);
                    }
                }
                string sql = $@"SELECT sum(precioCurso)
                            FROM cursos
                            WHERE idCurso in({modulos})";
                var valorTotalModulos = await _dapper.ExecuteScalarAsync<decimal>(sql);
                sql = $@"SELECT * FROM cursos
                        WHERE idCurso in({modulos})";
                var listaModulos = await _dapper.QueryAsync<cursos>(sql);
                _context.estudiantes.AddRange(listaEstudiantesGuardar);
                var descuento = await _context.tiposdescuentos.AsNoTracking().Where(x => x.idTipoDescuento == idTipoDescuento).FirstOrDefaultAsync();
                var dataGrupo = new gruposinhouse();
                dataGrupo.fechaRegistro = DateTime.Now;
                dataGrupo.idCliente = idCliente;
                dataGrupo.idTipoDescuento = descuento.idTipoDescuento;
                dataGrupo.porcentaje = descuento.porcentaje;
                dataGrupo.valorSinDescuento = valorTotalModulos * estudiantes.Count();
                dataGrupo.idGrupoCurso = _matricula.idGrupoCurso;
                dataGrupo.usuarioRegistro = _auth.getUser();
                var modulosGrupo = new List<gruposinhousemodulos>();
                foreach (var item in listaModulos)
                {
                    modulosGrupo.Add(new gruposinhousemodulos
                    {
                        idCurso = item.idCurso
                    });
                }
                dataGrupo.gruposinhousemodulos = modulosGrupo;
                _context.gruposinhouse.Add(dataGrupo);
                await _context.SaveChangesAsync();
                sql = @"SET @_idMatricula=0;";
                var usuarioRegistro = _auth.getUser();
                foreach (var item in estudiantes)
                {
                    var paralelo = paralelos.Where(x => x.documentoIdentidad == item.documentoIdentidad).Select(x => x.paralelo).FirstOrDefault();
                    sql += $@"INSERT INTO matriculas (idEstudiante, idCliente, idGrupoCurso, idTipoDescuento, paralelo,
                            fechaRegistro, esUniandes, idCarrera, idCentro, usuarioRegistro, legalizado, deuda, inHouse, idGrupoInHouse)
                            VALUES((SELECT idEstudiante FROM estudiantes WHERE documentoIdentidad='{item.documentoIdentidad}' limit 1),
                            {dataGrupo.idCliente}, @idGrupoCurso, {descuento.idTipoDescuento}, '{paralelo}',
                            current_timestamp(), 0, null, null, @usuarioRegistro, 1, 0, 1, @idGrupoInHouse);
                            SET @_idMatricula=LAST_INSERT_ID();";
                    foreach (var modulo in listaModulos)
                    {
                        sql += $@"
                            INSERT INTO calificaciones (idMatricula, idGrupoCurso, idCurso, fechaRegistro)
                            VALUES(@_idMatricula, @idGrupoCurso, {modulo.idCurso}, current_timestamp());";
                    }
                }
                await dapperMultiple.QueryMultipleAsync(sql, new { usuarioRegistro, _matricula.idGrupoCurso, dataGrupo.idGrupoInHouse });
                return Ok();
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
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
                return Ok(new { alumnos, modulos, info,pagos });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}