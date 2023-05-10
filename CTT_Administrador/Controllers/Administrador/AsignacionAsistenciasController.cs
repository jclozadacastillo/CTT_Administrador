using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace CTT_Administrador.Controllers.Administrador
{
    public class AsignacionAsistenciasController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;

        public AsignacionAsistenciasController(cttContext context, IAuthorizeAdministradorTools auth)
        {
            _auth = auth;
            _context = context;
        }

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
                                and c.activo =1 and g.activo=1
                                and p.activo = 1 and m.activa = 1
                                and datediff(current_date(),p.fechaInicio)>=0
                                and datediff(p.fechaFin,current_date())>=0
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

        [HttpPost]
        public async Task<IActionResult> comboCursos(int idPeriodo)
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
                                and c.activo =1 and g.activo=1
                                and p.activo = 1 and m.activa = 1
                                and datediff(current_date(),p.fechaInicio)>=0
                                and datediff(p.fechaFin,current_date())>=0
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

        [HttpPost]
        public async Task<IActionResult> comboCursosAsociados(int idGrupoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));

            try
            {
                string sql = @"
                                select distinct(c.idCurso) as idCurso,curso
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso=a.idGrupoCurso
                                inner join cursos_mallas m on m.idCurso = g.idCurso
                                inner join cursos c on c.idCurso = m.idCursoAsociado
                                where a.idGrupoCurso = @idGrupoCurso
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
                                order by a.paralelo
                ";
                return Ok(await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso }));
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

        public async Task<IActionResult> listar(int idGrupoCurso, int idCurso, string paralelo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                select c.*,e.documentoIdentidad,
                                concat(e.primerApellido,' ',e.segundoApellido,' ',e.primerNombre,' ',e.segundoNombre) as estudiante,
                                datediff(fechaLimiteNotas,current_timestamp()) as tiempoLimite,
                                datediff(fechaLimiteNotasAtraso,current_timestamp()) as tiempoLimiteAtraso
                                from matriculas m
                                inner join estudiantes e on e.idEstudiante = m.idEstudiante
                                inner join calificaciones c on c.idMatricula = m.idMatricula
                                inner join asignacionesinstructorescalificaciones a on a.idGrupoCurso = c.idGrupoCurso
                                and m.paralelo = a.paralelo and a.idCurso = c.idCurso
                                where m.idGrupoCurso = @idGrupoCurso and c.idCurso=@idCurso and m.paralelo=@paralelo
                                order by e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { idGrupoCurso, idCurso, paralelo });
                sql = @"SELECT * FROM cursos WHERE idCurso=@idCurso";
                var parametros = await dapper.QueryFirstOrDefaultAsync(sql, new { idCurso });
                sql = @"select i.* from asignacionesinstructorescalificaciones a
                    inner join instructores i on i.idInstructor = a.idInstructor
                    where idGrupoCurso =@idGrupoCurso
                    and idCurso=@idCurso
                    and paralelo=@paralelo
                    and a.activo=1 and i.activo=1";
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

        [HttpPost]
        public async Task<IActionResult> guardar([FromBody] calificaciones _data)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT numeroNotas,puntajeMinimo,asistenciaMinima,calificaAsistencia
                              from cursos WHERE idCurso=@idCurso"; ;
                var parametros = await dapper.QueryFirstOrDefaultAsync<cursos>(sql, _data);
                int? notas = parametros.numeroNotas;
                int? puntajeMinimo = parametros.puntajeMinimo;
                double acumulador = 0;
                for (var i = 1; i <= notas; i++)
                {
                    Type type = _data.GetType();
                    PropertyInfo info = type.GetProperty($"nota{i}");
                    acumulador += info.GetValue(_data) == null ? 0 : Convert.ToDouble(info.GetValue(_data));
                }
                _data.promedioFinal = Convert.ToDecimal(acumulador / notas);
                var aprobadoFaltas = true;
                if (parametros.calificaAsistencia == 1) aprobadoFaltas = _data.faltas >= parametros.asistenciaMinima;
                if (parametros.calificaAsistencia == 1) _data.promedioFinal = Convert.ToDecimal((_data.promedioFinal + _data.faltas) / 2);
                _data.aprobado = Convert.ToSByte(_data.promedioFinal >= Convert.ToDecimal(puntajeMinimo) && aprobadoFaltas);
                sql = @"
                                UPDATE calificaciones
                                SET faltas=@faltas,
                                aprobado=@aprobado,
                                justificaFaltas=@justificaFaltas,
                                justificacionObservacion=@justificacionObservacion
                                WHERE idMatricula=@idMatricula
                                AND idGrupoCurso=@idGrupoCurso
                                AND idCurso=@idCurso;
                              ";
                await dapper.ExecuteAsync(sql, _data);
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

        [HttpPost]
        public async Task<IActionResult> guardarTodo([FromBody] List<calificaciones> _data)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT numeroNotas,puntajeMinimo,asistenciaMinima,calificaAsistencia from cursos WHERE idCurso=@idCurso"; ;
                var parametros = await dapper.QueryFirstOrDefaultAsync<cursos>(sql, _data.FirstOrDefault());
                int? notas = parametros.numeroNotas;
                int? puntajeMinimo = parametros.puntajeMinimo;

                foreach (var item in _data)
                {
                    double acumulador = 0;
                    for (var i = 1; i <= notas; i++)
                    {
                        Type type = item.GetType();
                        PropertyInfo info = type.GetProperty($"nota{i}");
                        acumulador += info.GetValue(item) == null ? 0 : Convert.ToDouble(info.GetValue(item));
                    }
                    item.promedioFinal = Convert.ToDecimal(acumulador / notas);
                    var aprobadoFaltas = true;
                    if (parametros.calificaAsistencia == 1) item.promedioFinal = Convert.ToDecimal((item.promedioFinal + item.faltas) / 2);
                    if (parametros.calificaAsistencia == 1) aprobadoFaltas = item.faltas >= parametros.asistenciaMinima;
                    item.aprobado = Convert.ToSByte(item.promedioFinal >= Convert.ToDecimal(puntajeMinimo) && aprobadoFaltas);
                    _context.calificaciones.Update(item);
                }
                await _context.SaveChangesAsync();
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
    }
}