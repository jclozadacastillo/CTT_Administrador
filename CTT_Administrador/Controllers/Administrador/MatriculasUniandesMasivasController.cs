using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Rotativa.AspNetCore;

namespace CTT_Administrador.Controllers.Administrador
{
    public class MatriculasUniandesMasivasController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;

        public MatriculasUniandesMasivasController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
        {
            _context = context;
            _auth = auth;
            _path = _env.WebRootPath;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> comboPeriodos()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                SELECT distinct(p.idPeriodo),p.detalle
                FROM gruposcursos g
                inner join cursos c on c.idCurso = g.idCurso
                inner join periodos p on p.idPeriodo = g.idPeriodo
                inner join modalidades m on m.idModalidad = g.idModalidad
                inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                where
                (c.idCurso in(
                select idCurso
                from cursos_mallas cm
                where cm.idCursoAsociado = c.idCurso)
                or
                c.idCurso not in(
                select idCursoAsociado
                from cursos_mallas cm))
                and c.activo =1 and g.activo=1
                and p.activo = 1 and m.activa = 1
                and datediff(current_date(),p.fechaInicio)>=0
                and datediff(p.fechaFin,current_date())>=0
                                and (c.esVisible = 0 or c.esVisible is null)
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
        public async Task<IActionResult> comboTiposCursos(int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                SELECT distinct(t.idTipoCurso),t.tipoCurso
                FROM gruposcursos g
                inner join cursos c on c.idCurso = g.idCurso
                inner join periodos p on p.idPeriodo = g.idPeriodo
                inner join modalidades m on m.idModalidad = g.idModalidad
                inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                where
                (c.idCurso in(
                select idCurso
                from cursos_mallas cm
                where cm.idCursoAsociado = c.idCurso)
                or
                c.idCurso not in(
                select idCursoAsociado
                from cursos_mallas cm))
                and c.activo =1 and g.activo=1
                and p.activo = 1 and m.activa = 1
                and datediff(current_date(),p.fechaInicio)>=0
                and datediff(p.fechaFin,current_date())>=0
                                and (c.esVisible = 0 or c.esVisible is null)
                and p.idPeriodo=@idPeriodo
                ";
                return Ok(await dapper.QueryAsync(sql, new { idPeriodo }));
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
        public async Task<IActionResult> comboCursos(int idTipoCurso, int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                            SELECT distinct(g.idGrupoCurso),concat(c.curso,' (',m.modalidad,')') as curso,
                            c.curso as cursoSolo
                            FROM gruposcursos g
                            inner join cursos c on c.idCurso = g.idCurso
                            inner join periodos p on p.idPeriodo = g.idPeriodo
                            inner join modalidades m on m.idModalidad = g.idModalidad
                            inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                            where
                            (c.idCurso in(
                            select idCurso
                            from cursos_mallas cm
                            where cm.idCursoAsociado = c.idCurso)
                            or
                            c.idCurso not in(
                            select idCursoAsociado
                            from cursos_mallas cm))
                            and c.activo =1 and g.activo=1
                            and p.activo = 1 and m.activa = 1
                            and datediff(current_date(),p.fechaInicio)>=0
                            and datediff(p.fechaFin,current_date())>=0
                                            and (c.esVisible = 0 or c.esVisible is null)
                and p.idPeriodo=@idPeriodo and t.idTipoCurso=@idTipoCurso
                ";
                return Ok(await dapper.QueryAsync(sql, new { idTipoCurso, idPeriodo }));
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

        public class centros
        {
            public string idcentro { get; set; }
            public string centro_detalle { get; set; }
        }

        public class carreras
        {
            public string codigo_carrera { get; set; }
            public string carrera { get; set; }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> cargarDatos(IFormFile archivo)
        {
            try
            {
                string novedades = "";
                Stream stream = archivo.OpenReadStream();
                IWorkbook excelFile = Path.GetExtension(archivo.FileName).ToLower() == ".xlsx" ?
                new XSSFWorkbook(stream) : new HSSFWorkbook(stream);
                ISheet hojaDatos = excelFile.GetSheetAt(0);
                ISheet hojaCentros = excelFile.GetSheetAt(1);
                ISheet hojaCarreras = excelFile.GetSheetAt(2);
                List<centros> listaCentros = new List<centros>();
                List<carreras> listaCarreras = new List<carreras>();
                List<dynamic> listaEstudiantes = new List<dynamic>();
                foreach (IRow item in hojaCentros)
                {
                    if (!string.IsNullOrEmpty(item.GetCell(0).ToString())
                        && !string.IsNullOrEmpty(item.GetCell(1).ToString()))
                    {
                        listaCentros.Add(new centros
                        {
                            idcentro = item.GetCell(0).ToString(),
                            centro_detalle = item.GetCell(1).ToString()
                        });
                    }
                }

                foreach (IRow item in hojaCarreras)
                {
                    if (!string.IsNullOrEmpty(item.GetCell(0).ToString())
                        && !string.IsNullOrEmpty(item.GetCell(1).ToString()))
                    {
                        listaCarreras.Add(new carreras
                        {
                            codigo_carrera = item.GetCell(0).ToString(),
                            carrera = item.GetCell(1).ToString()
                        });
                    }
                }

                for (int i = 8; i < hojaDatos.LastRowNum; i++)
                {
                    var item = hojaDatos.GetRow(i);
                    var referencia = item.GetCell(1).ToString();
                    if (string.IsNullOrEmpty(item.GetCell(0).ToString())
                        || string.IsNullOrEmpty(item.GetCell(1).ToString())
                        || string.IsNullOrEmpty(item.GetCell(3).ToString())
                        //|| string.IsNullOrEmpty(item.GetCell(5).ToString())
                        //|| string.IsNullOrEmpty(item.GetCell(6).ToString())
                        )
                    {
                        if (item.GetCell(0).ToString().Trim() == "" && item.GetCell(1).ToString().Trim() != "")
                            novedades += $"<li>Se ha <b>omitido</b> la fila <b>{i + 1}</b> por que no tiene documento de identidad.</li>";
                    }
                    else
                    {
                        listaEstudiantes.Add(new
                        {
                            idTipoDocumento = validarCedula(item.GetCell(0).ToString()) ? "C" : "P",
                            documentoIdentidad = string.IsNullOrEmpty(item.GetCell(0)?.ToString()) ? "" : item.GetCell(0)?.ToString()?.Trim(),
                            primerNombre = string.IsNullOrEmpty(item.GetCell(1)?.ToString()) ? "" : item.GetCell(1)?.ToString()?.TrimStart()?.TrimEnd(),
                            segundoNombre = string.IsNullOrEmpty(item.GetCell(2)?.ToString()) ? "" : item.GetCell(2)?.ToString()?.TrimStart()?.TrimEnd(),
                            primerApellido = string.IsNullOrEmpty(item.GetCell(3)?.ToString()) ? "" : item.GetCell(3)?.ToString()?.TrimStart()?.TrimEnd(),
                            segundoApellido = string.IsNullOrEmpty(item.GetCell(4)?.ToString()) ? "" : item.GetCell(4)?.ToString()?.TrimStart()?.TrimEnd(),
                            centro_detalle = string.IsNullOrEmpty(item.GetCell(5)?.ToString()) ? "" : item.GetCell(5)?.ToString()?.TrimStart()?.TrimEnd(),
                            direccion = string.IsNullOrEmpty(item.GetCell(5)?.ToString()) ? "" : item.GetCell(5)?.ToString()?.TrimStart()?.TrimEnd(),
                            listaCentros.Where(x => x.centro_detalle == item.GetCell(5)?.ToString()).FirstOrDefault()?.idcentro,
                            carrera = string.IsNullOrEmpty(item.GetCell(6)?.ToString()) ? "" : item.GetCell(6)?.ToString(),
                            listaCarreras.Where(x => x.carrera == item.GetCell(6)?.ToString()).FirstOrDefault()?.codigo_carrera,
                            activo = 1
                        });
                    }
                }

                return Ok(new { novedades, listaEstudiantes });
            }
            catch (Exception ex)
            {
                return Problem($"<b>Problema al procesar archivo:</b> {ex.Message}");
            }
        }

        public class alumnosMigracion
        {
            public string documentoIdentidad { get; set; }
            public string? idcentro { get; set; }
            public string? centro_detalle { get; set; }
            public string? codigo_carrera { get; set; }
            public string? carrera { get; set; }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> generarMatriculas(string _alumnos, matriculas _data, string _alumnosCedulas)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                var usuario = _auth.getUser();
                var listaAlumnosMigracion = JsonConvert.DeserializeObject<List<alumnosMigracion>>(_alumnos.ToUpper());
                var listaEstudiantes = JsonConvert.DeserializeObject<List<estudiantes>>(_alumnos.ToUpper());
                var listaClientes = JsonConvert.DeserializeObject<List<clientesfacturas>>(_alumnos.ToUpper());

                string sql = $@"
                               SELECT documentoIdentidad
                               FROM estudiantes
                               WHERE documentoIdentidad in({_alumnosCedulas});
                              ";
                var existentesEstudiantes = await dapper.QueryAsync<string>(sql);
                listaEstudiantes = listaEstudiantes.Where(x => !(existentesEstudiantes.ToList().Contains(x.documentoIdentidad))).ToList();
                if (listaEstudiantes.Count > 0)
                {
                    _context.estudiantes.AddRange(listaEstudiantes);
                    await _context.SaveChangesAsync();
                }

                sql = $@"
                               SELECT documento
                               FROM clientesfacturas
                               WHERE documento in({_alumnosCedulas});
                              ";
                var existentesClientes = await dapper.QueryAsync<string>(sql);
                listaClientes = listaClientes.Where(x => !(existentesClientes.ToList().Contains(x.documento))).ToList();
                if (listaClientes.Count > 0)
                {
                    _context.clientesfacturas.AddRange(listaClientes);
                    await _context.SaveChangesAsync();
                }

                sql = $@"
                        INSERT INTO matriculas
                        (idEstudiante, idCliente, idGrupoCurso, idTipoDescuento, paralelo,
                        fechaRegistro, esUniandes, idCarrera, idCentro, usuarioRegistro,legalizado)
                        select distinct(e.idEstudiante),
                        (select cf.idCliente FROM clientesfacturas cf
                        WHERE  cf.documento=e.documentoIdentidad limit 1) as idCliente,
                        @idGrupoCurso,1 as idTipoDescuento,@paralelo,
                        current_timestamp(),1,null,null,@usuario,1
                        from estudiantes e
                        where e.documentoIdentidad in({_alumnosCedulas})
                        and e.idEstudiante not in(select m.idEstudiante
                        from matriculas m
                        inner join gruposcursos g on m.idGrupoCurso = @idGrupoCurso
                        );
                        ";
                await dapper.ExecuteAsync(sql, new { _data.idGrupoCurso, usuario, _data.paralelo });
                sql = "";
                dapper.Close();
                dapper.Open();
                foreach (var item in listaAlumnosMigracion)
                {
                    sql += $@"
                                update matriculas m
                                inner join estudiantes e on e.idEstudiante =m.idEstudiante
                                set m.idCarrera='{item.codigo_carrera}',m.idCentro='{item.idcentro}'
                                where idGrupoCurso = {_data.idGrupoCurso} and e.documentoIdentidad ='{item.documentoIdentidad}';
                        ";
                }
                await dapper.QueryMultipleAsync(sql);
                dapper.Close();
                dapper.Open();
                sql = @"
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
                        select c2.idCurso from cursos c2 where activo=1
                        )
                        and concat(cast(m.idMatricula as char),'_',
                        cast(m.idGrupoCurso as char),'_',cast(cm.idCursoAsociado as char))
                        not in(
                        select concat(cast(idMatricula as char),'_',
                        cast(idGrupoCurso as char),'_',cast(idCurso as char))
                        from calificaciones ca where ca.idMatricula=m.idMatricula
                        ) and m.idGrupoCurso = @idGrupoCurso
                        and m.usuarioRegistro=@usuario
                        and m.paralelo=@paralelo
                    ";
                await dapper.ExecuteAsync(sql, new { _data.idGrupoCurso, usuario, _data.paralelo });
                dapper.Close();

                //Creditos
                //dapper.Open();
                //sql = $@"SELECT DISTINCT(ca.idMatricula),ca.idCurso,c.precioCurso
                //        FROM calificaciones ca
                //        INNER JOIN cursos c ON c.idCurso = ca.idCurso
                //        INNER JOIN matriculas m ON m.idMatricula = ca.idMatricula
                //        INNER JOIN estudiantes e ON e.idEstudiante = m.idEstudiante
                //        WHERE ca.idGrupoCurso = @idGrupoCurso
                //        AND m.paralelo=@paralelo
                //        AND c.precioCurso >0
                //        AND e.documentoIdentidad in({_alumnosCedulas})";

                //var listaCreditos = await dapper.QueryAsync(sql, new { _data.idGrupoCurso, _data.paralelo });
                //if(listaCreditos.Count()> 0)
                //{
                //    foreach (var item in listaCreditos)
                //    {
                //        sql = @"SELECT idCredito FROM creditos WHERE idMatricula=@idMatricula";
                //        int idCredito = await dapper.ExecuteScalarAsync<int>(sql, new { item.idMatricula });
                //        idCredito = idCredito == null ? 0 : idCredito;
                //        if (idCredito == 0)
                //        {
                //            sql = @"INSERT INTO creditos (fechaCredito, idMatricula, activo, fechaDesactivacion)
                //                    VALUES(current_timestamp(),@idMatricula, 1, null);
                //                    SELECT LAST_INSERT_ID();";
                //            idCredito = await dapper.ExecuteScalarAsync<int>(sql, new { item.idMatricula });
                //        }
                //        if (idCredito > 0)
                //        {
                //            sql = $@"INSERT INTO detallecreditos (idCredito,idCurso,valor,valorPendiente,cancelado,activo,fechaDesactivacion,fechaRegistro)
                //            SELECT @idCredito,idCurso,precioCurso,precioCurso,0,1,null,current_timestamp() FROM cursos
                //            WHERE idCurso in({item.idCurso})";
                //            await dapper.ExecuteAsync(sql, new { idCredito });
                //        }
                //    }

                //}
                //dapper.Close();
                return generarPdfReporte(_data, _alumnosCedulas, usuario);
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
        public IActionResult generarPdfReporte(matriculas _data, string _alumnosCedulas, string usuario)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = $@"SELECT c.curso,tp.tipoCurso,
                                p.detalle,'{_data.paralelo}' as paralelo,
                                current_timestamp() as fechaRegistro
                                FROM gruposcursos g
                                INNER JOIN cursos c on c.idCurso=g.idCurso
                                INNER JOIN tiposcursos tp on tp.idTipoCurso=c.idTipoCurso
                                INNER JOIN periodos p on p.idPeriodo=g.idPeriodo
                                WHERE g.idGrupoCurso=@idGrupoCurso
                              ";
                var datosCurso = dapper.QueryFirstOrDefault(sql, _data);
                sql = $@"SELECT e.documentoIdentidad,e.primerApellido,
                         e.segundoApellido,e.primerNombre,e.segundoNombre,
                         m.idCarrera,m.idCentro,m.fechaRegistro,m.usuarioRegistro
                         FROM estudiantes e
                         LEFT JOIN matriculas m on e.idEstudiante=m.idEstudiante AND m.idGrupoCurso=@idGrupoCurso
                         AND usuarioRegistro=@usuario AND  m.paralelo=@paralelo
                         WHERE e.documentoIdentidad in({_alumnosCedulas})
                        ";
                var listaMatriculados = dapper.Query(sql, new { _data.idGrupoCurso, usuario, _data.paralelo });
                var data = new { datosCurso, listaMatriculados, path = _path, error = "" };
                return new ViewAsPdf("pdfReporte", data)
                {
                    FileName = "reporte.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    CustomSwitches = "--disable-smart-shrinking",
                    PageMargins = { Top = 5, Right = 5, Bottom = 5, Left = 5 }
                };
            }
            catch (Exception ex)
            {
                var data = new
                {
                    error = ex.Message,
                    listaMatriculados = new List<dynamic>(),
                    datosCurso = new { },
                    path = _path
                };
                return new ViewAsPdf("pdfReporte", data)
                {
                    FileName = "reporte.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    CustomSwitches = "--disable-smart-shrinking",
                    PageMargins = { Top = 5, Right = 5, Bottom = 5, Left = 5 }
                };
            }
            finally
            {
                dapper.Dispose();
            }
        }

        public IActionResult pdfReporte()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Response.Cookies.Append("errorRotativa", ex.Message);
                return RedirectToAction("Error", "Error");
            }
        }

        private bool validarCedula(string ced)
        {
            try
            {
                int isNumeric;
                var total = 0;
                const int tamanoLongitudCedula = 10;
                int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
                const int numeroProvincias = 24;
                const int tercerDigito = 6;

                if (int.TryParse(ced, out isNumeric) && ced.Length == tamanoLongitudCedula)
                {
                    var provincia = Convert.ToInt32(string.Concat(ced[0], ced[1], string.Empty));
                    var digitoTres = Convert.ToInt32(ced[2] + string.Empty);
                    if ((provincia > 0 && provincia <= numeroProvincias) && digitoTres < tercerDigito)
                    {
                        var digitoVerificadorRecibido = Convert.ToInt32(ced[9] + string.Empty);
                        for (var k = 0; k < coeficientes.Length; k++)
                        {
                            var valor = Convert.ToInt32(coeficientes[k] + string.Empty) *
                            Convert.ToInt32(ced[k] + string.Empty);
                            total = valor >= 10 ? total + (valor - 9) : total + valor;
                        }
                        var digitoVerificadorObtenido = total >= 10 ? (total % 10) != 0 ?
                        10 - (total % 10) : (total % 10) : total;
                        return digitoVerificadorObtenido == digitoVerificadorRecibido;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}