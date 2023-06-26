using ArrayToExcel;
using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Rotativa.AspNetCore;

namespace CTT_Administrador.Controllers.Administrador
{
    public class AdministrarMatriculasController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;

        public AdministrarMatriculasController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
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
                return Ok(await dapper.QueryAsync(sql, new {idPeriodo}));
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
        public async Task<IActionResult> comboCursos(int idPeriodo, int idTipoCurso)
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
                                and p.idPeriodo=@idPeriodo
                                and c.idTipoCurso=@idTipoCurso
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
        public async Task<IActionResult> comboCursosAsociados(int idGrupoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));

            try
            {
                string sql = @"
                                select distinct(c.idCurso) as idCurso,curso
                                from gruposcursos g
                                inner join cursos_mallas m on m.idCurso = g.idCurso
                                inner join cursos c on c.idCurso = m.idCursoAsociado
                                where g.idGrupoCurso = @idGrupoCurso
                ";
                return Ok(await dapper.QueryAsync(sql, new { idGrupoCurso }));
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
        public async Task<IActionResult> comboParalelos(int idGrupoCurso, int idCurso)
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
                                where a.idGrupoCurso = @idGrupoCurso and a.idCurso=@idCurso
                                and a.activo=1
                                order by a.paralelo
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
        public async Task<IActionResult> listar(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = $@"
                                select c.*,e.documentoIdentidad,
                                concat(e.primerApellido,' ',e.segundoApellido,' ',e.primerNombre,' ',e.segundoNombre) as estudiante,
                                datediff(fechaLimiteNotas,current_timestamp()) as tiempoLimite,
                                datediff(fechaLimiteNotasAtraso,current_timestamp()) as tiempoLimiteAtraso,a.paralelo
                                from matriculas m
                                inner join estudiantes e on e.idEstudiante = m.idEstudiante
                                inner join calificaciones c on c.idMatricula = m.idMatricula
                                inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                                and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                                where m.idGrupoCurso = @idGrupoCurso and a.activo=1 and c.idCurso=@idCurso {paralelos}
                                order by a.paralelo,e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso, paralelo });
                sql = @"SELECT * FROM cursos WHERE idCurso=@idCurso";
                var parametros = await dapper.QueryFirstOrDefaultAsync(sql, new { idCurso });
                sql = @"select i.* from asignacionesinstructorescalificaciones a
                    inner join instructores i on i.idInstructor = a.idInstructor
                    where idGrupoCurso =@idGrupoCurso
                    and idCurso=@idCurso
                    and paralelo=@paralelo
                    and a.activo=1";
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
                    and idCurso=@idCurso
                    and paralelo=@paralelo
                    and a.activo=1";
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
        public async Task<IActionResult> suspender(int idGrupoCurso, int idCurso, int idMatricula)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"UPDATE calificaciones
                               set suspendido=case when suspendido=1 then 0 else 1 end
                               WHERE idCurso=@idCurso
                               AND idMatricula=@idMatricula
                               AND idGrupoCurso=@idGrupoCurso
                              ";
                await dapper.ExecuteAsync(sql, new { idCurso, idMatricula, idGrupoCurso });
                return Ok();
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
        public async Task<IActionResult> eliminar(int idGrupoCurso, int idCurso, int idMatricula)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"DELETE FROM calificaciones
                               WHERE idCurso=@idCurso
                               AND idMatricula=@idMatricula
                               AND idGrupoCurso=@idGrupoCurso
                              ";
                await dapper.ExecuteAsync(sql, new { idCurso, idMatricula, idGrupoCurso });
                return Ok();
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
        public async Task<IActionResult> generarExcel(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = @"select numeroNotas from cursos where idCurso=@idCurso";
                int cantidadNotas = await dapper.ExecuteScalarAsync<int>(sql, new { idCurso });
                string notas = "";
                for (int i = 1; i <= cantidadNotas; i++)
                {
                    notas += $",nota{i}";
                }

                sql = $@"select cen.centro,g.fechaInicioCurso,g.fechaFinCurso,cu.horasCurso as duracion,e.documentoIdentidad,
                         e.primerApellido,e.segundoApellido,concat(e.primerNombre,' ',e.segundoNombre) as nombres,
                         ca.carrera,
                         a.paralelo{notas},c.faltas,c.promedioFinal,
                         case when c.justificaFaltas=1 then 'SI' else 'NO' end as justificaFaltas,
                         case when c.justificaFaltas = 1 then 'JUSTIFICADO'
                         when c.justificaFaltas != 1 and c.aprobado =0 and suspendido != 1 then 'REPROBADO'
                         when c.suspendido = 1 then 'SUSPENDIDO'
                         when c.aprobado = 1 then 'APROBADO' end as estado
                         from matriculas m
                         inner join estudiantes e on e.idEstudiante = m.idEstudiante
                         inner join calificaciones c on c.idMatricula = m.idMatricula
                         inner join cursos cu on cu.idCurso=@idCurso
                         left join carrerasuniandes ca on ca.idCarrera=m.idCarrera
                         left join centrosuniandes cen on cen.idCentro=m.idCentro
                         inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                         inner join gruposcursos g on g.idGrupoCurso=a.idGrupoCurso
                         and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                         where m.idGrupoCurso = @idGrupoCurso and c.idCurso=@idCurso {paralelos}
                         order by a.paralelo,e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso, paralelo });
                var excel = listaCalificaciones.ToExcel(op =>
                {
                    op.SheetName("MATRICULAS");
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
        public async Task<IActionResult> generarExcelCertificados(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = @"select numeroNotas from cursos where idCurso=@idCurso";
                int cantidadNotas = await dapper.ExecuteScalarAsync<int>(sql, new { idCurso });
                string notas = "";
                for (int i = 1; i <= cantidadNotas; i++)
                {
                    notas += $",nota{i}";
                }

                sql = $@"select e.documentoIdentidad as 'Cédula',
                         e.primerNombre,e.segundoNombre,e.primerApellido,e.segundoApellido,
                         cen.centro as 'Ciudad',ca.carrera as 'Carrera'
                         from matriculas m
                         inner join estudiantes e on e.idEstudiante = m.idEstudiante
                         inner join calificaciones c on c.idMatricula = m.idMatricula
                         inner join cursos cu on cu.idCurso=@idCurso
                         left join carrerasuniandes ca on ca.idCarrera=m.idCarrera
                         left join centrosuniandes cen on cen.idCentro=m.idCentro
                         inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                         and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                         where m.idGrupoCurso = @idGrupoCurso and a.activo=1 and c.idCurso=@idCurso {paralelos}
                         and aprobado=1
                         order by a.paralelo,e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso, paralelo });
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
        public IActionResult generarPdfReporte(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string paralelos = paralelo == "TODOS" ? "" : "and m.paralelo = @paralelo";
                string sql = @"select numeroNotas from cursos where idCurso=@idCurso";
                int cantidadNotas = dapper.ExecuteScalar<int>(sql, new { idCurso });
                sql = $@"SELECT c.curso,i.documentoIdentidad,i.primerApellido,i.segundoApellido,  
                        i.primerNombre, i.segundoNombre,g.fechaInicioCurso,g.fechaFinCurso,c.horasCurso,a.paralelo,
                        i.abreviaturaTitulo
                        FROM cursos c
                        INNER JOIN gruposcursos g ON g.idCurso = c.idCurso 
                        INNER JOIN asignacionesinstructorescalificaciones a ON a.idGrupoCurso = g.idGrupoCurso  
                        INNER JOIN instructores i ON i.idInstructor = a.idInstructor 
                        WHERE a.idGrupoCurso = @idGrupoCurso AND a.idCurso = @idCurso {paralelos.Replace("m.paralelo","a.paralelo")}
                        ORDER BY a.paralelo
                        ";
                var cursos = dapper.Query(sql, new { idGrupoCurso,idCurso,paralelo });
                var curso = cursos.FirstOrDefault();
                string notas = "";
                for (int i = 1; i <= cantidadNotas; i++)
                {
                    notas += $",nota{i}";
                }

                sql = $@"select cen.centro,e.documentoIdentidad,
                         e.primerApellido,e.segundoApellido,concat(e.primerNombre,' ',e.segundoNombre) as nombres,
                         a.paralelo,c.promedioFinal{notas},c.faltas,
                         ca.carrera,
                         case when c.justificaFaltas=1 then 'SI' else 'NO' end as justificaFaltas,
                         case when c.justificaFaltas = 1 then 'JUSTIFICADO'
                         when c.justificaFaltas != 1 and c.aprobado =0 and suspendido != 1 then 'REPROBADO'
                         when c.suspendido = 1 then 'SUSPENDIDO'
                         when c.aprobado = 1 then 'APROBADO' end as estado
                         from matriculas m
                         inner join estudiantes e on e.idEstudiante = m.idEstudiante
                         inner join calificaciones c on c.idMatricula = m.idMatricula
                         inner join cursos cu on cu.idCurso=@idCurso
                         left join carrerasuniandes ca on ca.idCarrera=m.idCarrera
                         left join centrosuniandes cen on cen.idCentro=m.idCentro
                         inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                         and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                         where m.idGrupoCurso = @idGrupoCurso and a.activo=1 and c.idCurso=@idCurso {paralelos}
                         order by e.primerApellido,e.segundoApellido,e.primerNombre
                ";
                var listado = dapper.Query(sql, new { idCurso, idGrupoCurso, paralelo });
                return new ViewAsPdf("pdfReporte", new { error = "", listado, path = _path, cantidad = listado.Count(), cursos, curso })
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
                    cursos = new List<dynamic>(),
                    curso=new {},
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