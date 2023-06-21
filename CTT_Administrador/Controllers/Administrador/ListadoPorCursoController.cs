using ArrayToExcel;
using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Dynamic;

namespace CTT_Administrador.Controllers.Administrador
{
    public class ListadoPorCursoController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;

        public ListadoPorCursoController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
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
                                select distinct(p.idPeriodo),detalle
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso = a.idGrupoCurso
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
                                select distinct(t.idTipoCurso),t.tipoCurso
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso = a.idGrupoCurso
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
        public async Task<IActionResult> comboCursos(int idPeriodo,int idTipoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                select distinct(g.idGrupoCurso),concat(curso,' (',m.modalidad,')') as curso
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso = a.idGrupoCurso
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
                                and p.idPeriodo=@idPeriodo and t.idTipoCurso=@idTipoCurso
                ";
                return Ok(await dapper.QueryAsync(sql, new { idPeriodo,idTipoCurso }));
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
        public async Task<IActionResult> comboParalelos(int idGrupoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                select distinct(a.paralelo)
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso=a.idGrupoCurso
                                inner join cursos_mallas m on m.idCurso = g.idCurso
                                inner join cursos c on c.idCurso = m.idCursoAsociado
                                where a.idGrupoCurso = @idGrupoCurso
                                order by a.paralelo
                ";
                var paralelos = new List<dynamic>() { new { paralelo = "TODOS" } };
                paralelos.AddRange(await dapper.QueryAsync(sql, new { idGrupoCurso }));
                return Ok(paralelos);
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
        public async Task<IActionResult> comboParalelosListado(int idGrupoCurso, int idCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                select distinct(m.paralelo)
                                from calificaciones ca
                                inner join Matriculas m on m.idMatricula=ca.idMatricula
                                where ca.idGrupoCurso = @idGrupoCurso and ca.idCurso=@idCurso
                                order by m.paralelo
                ";
                var paralelos = new List<dynamic>() { new { paralelo = "TODOS" } };
                paralelos.AddRange(await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso }));
                return Ok(paralelos);
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
        public async Task<IActionResult> listar(int idGrupoCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = $@"SELECT m.idCurso,m.curso  
                        FROM gruposcursos g
                        INNER JOIN cursos c ON c.idCurso = g.idCurso 
                        INNER JOIN cursos_mallas cm ON cm.idCurso = c.idCurso 
                        INNER JOIN cursos m ON m.idCurso = cm.idCursoAsociado 
                        WHERE idGrupoCurso = @idGrupoCurso
                        ORDER BY m.numeroModulo";
                var listaModulos = await dapper.QueryAsync(sql, new { idGrupoCurso });
                //var modulos = ",0 as 'promedioGeneral'";
                //foreach (var item in listaModulos)
                //{
                //    modulos += $" ,0 as 'curso{item.idCurso}' ";
                //}
                sql = $@"       select *,
                                (SELECT c.carrera 
                                FROM matriculas m
                                LEFT JOIN carrerasuniandes c ON c.idCarrera = m.idCarrera 
                                WHERE m.idMatricula=t1.idMatricula
                                LIMIT 1
                                ) as carrera,
                                (SELECT ce.centro 
                                FROM matriculas m
                                LEFT JOIN centrosuniandes ce ON ce.idCentro = m.idCentro 
                                WHERE m.idMatricula=t1.idMatricula
                                LIMIT 1
                                ) as centro
                                from 
                                (select distinct(m.idMatricula),e.documentoIdentidad,
                                e.primerApellido,e.segundoApellido,e.primerNombre,e.segundoNombre,
                                a.paralelo
                                from matriculas m
                                inner join estudiantes e on e.idEstudiante = m.idEstudiante
                                inner join calificaciones c on c.idMatricula = m.idMatricula
                                inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                                and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                                where m.idGrupoCurso = @idGrupoCurso {paralelos}) t1
                                order by paralelo,primerApellido,segundoApellido,primerNombre
                ";
                var listaEstudiantes = await dapper.QueryAsync(sql, new { idGrupoCurso, paralelo });

          
                sql = $@"
                        SELECT c.* FROM calificaciones c
                        INNER JOIN matriculas m ON m.idMatricula = c.idMatricula 
                        WHERE c.idGrupoCurso = @idGrupoCurso {paralelos}
                        ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, paralelo });
                sql = @"SELECT * FROM cursos";
                var parametros = await dapper.QueryFirstOrDefaultAsync(sql);
                sql = @"select i.* from asignacionesinstructorescalificaciones a
                    inner join instructores i on i.idInstructor = a.idInstructor
                    where idGrupoCurso =@idGrupoCurso
                    and paralelo=@paralelo and 1!=1";
                var instructor = await dapper.QueryFirstOrDefaultAsync(sql, new { idGrupoCurso, paralelo });
                return Ok(new { listaEstudiantes,listaCalificaciones,listaModulos, parametros, instructor });
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
        public async Task<IActionResult> listarMatriculados(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = $@"
                                select c.*,e.documentoIdentidad,
                                concat(e.primerApellido,' ',e.segundoApellido,' ',e.primerNombre,' ',e.segundoNombre) as estudiante,
                                0 as tiempoLimite,
                                0 as tiempoLimiteAtraso,m.paralelo
                                from matriculas m
                                inner join estudiantes e on e.idEstudiante = m.idEstudiante
                                inner join calificaciones c on c.idMatricula = m.idMatricula
                                where m.idGrupoCurso = @idGrupoCurso and c.idCurso=@idCurso {paralelos}
                                order by m.paralelo,e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso, paralelo });
                sql = @"SELECT * FROM cursos WHERE idCurso=@idCurso";
                var parametros = await dapper.QueryFirstOrDefaultAsync(sql, new { idCurso });
                sql = @"select i.* from asignacionesinstructorescalificaciones a
                    inner join instructores i on i.idInstructor = a.idInstructor
                    where idGrupoCurso =@idGrupoCurso
                    and paralelo=@paralelo and 1!=1";
                var instructor = await dapper.QueryFirstOrDefaultAsync(sql, new { idCurso, idGrupoCurso, paralelo });
                return Ok(new { listaCalificaciones, parametros, instructor });
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
        public async Task<IActionResult> generarExcel(string _lista)
        {
            try
            {
                var listaInicial = JsonConvert.DeserializeObject<List<dynamic>>(_lista);
                var listaCalificaciones=new List<dynamic>();
                foreach (var item in listaInicial)
                {
                    dynamic objeto = new System.Dynamic.ExpandoObject();
                    foreach (var tipo in item)
                    {
                        AddProperty(objeto, tipo.Name.ToString().ToUpper(), tipo.Value.Value);
                    }
                    listaCalificaciones.Add(objeto);
                }
                var excel = listaCalificaciones.ToExcel(op =>
                {
                    op.SheetName("CONSOLIDADO_NOTAS");
                });
                var archivo = new FileContentResult(excel, "application/vnd.ms-excel");
                archivo.FileDownloadName = $"REPORTE_{DateTime.Now.Ticks}.xlsx";
                return archivo;
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> generarExcelCertificados(string _lista)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                var listaInicial = JsonConvert.DeserializeObject<List<dynamic>>(_lista);
                var listaCalificaciones = new List<dynamic>();
                foreach (var item in listaInicial)
                {
                    dynamic objeto = new System.Dynamic.ExpandoObject();
                    foreach (var tipo in item)
                    {
                        AddProperty(objeto, tipo.Name, tipo.Value.Value);
                    }
                    listaCalificaciones.Add(objeto);
                }
                var excel = listaCalificaciones.ToExcel(op =>
                {
                    op.SheetName("CERTIFICADOS");
                });


                var archivo = new FileContentResult(excel, "application/vnd.ms-excel");
                archivo.FileDownloadName = $"REPORTE_{DateTime.Now.Ticks}.xlsx";
                return archivo;
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
        public IActionResult generarPdfReporte(int idGrupoCurso,string _lista)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT c.curso  FROM gruposcursos g
                            INNER JOIN cursos c ON c.idCurso = g.idCurso 
                            WHERE idGrupoCurso =@idGrupoCurso";
                var curso = dapper.ExecuteScalar<string>(sql, new { idGrupoCurso });
                var listaInicial = JsonConvert.DeserializeObject<List<dynamic>>(_lista);
                var listado = new List<dynamic>();
                foreach (var item in listaInicial)
                {
                    dynamic objeto = new System.Dynamic.ExpandoObject();
                    foreach (var tipo in item)
                    {
                        AddProperty(objeto, tipo.Name, tipo.Value.Value);
                    }
                    listado.Add(objeto);
                }
                return new ViewAsPdf("pdfReporte", new { error = "", listado, path = _path, cantidad = listado.Count(), curso })
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
                    listado = new List<dynamic>(),
                    path = _path,
                    curso = "",
                    cantidad = 0
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
    }
}