using System.Reflection;
using CTT_Administrador.Auth.Docente;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeDocenteTools _auth;
        public CalificacionesController(cttContext context,IAuthorizeDocenteTools auth)
        {
            _auth = auth;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> comboPeriodos()
        {
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            var usuario = _auth.getUser();
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
                                and a.idInstructor = @usuario
                ";
                return Ok(await dapper.QueryAsync(sql, new {usuario}));
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
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            var usuario = _auth.getUser();
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
                                and a.idInstructor = @usuario and p.idPeriodo=@idPeriodo
                ";
                return Ok(await dapper.QueryAsync(sql, new { usuario,idPeriodo }));
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
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            var usuario = _auth.getUser();
            try
            {
                string sql = @"
                                select c.idCurso,curso 
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso=a.idGrupoCurso
                                inner join cursos_mallas m on m.idCurso = g.idCurso 
                                inner join cursos c on c.idCurso = m.idCursoAsociado 
                                where a.idGrupoCurso = @idGrupoCurso and a.idInstructor=@usuario
                ";
                return Ok(await dapper.QueryAsync(sql, new { usuario, idGrupoCurso }));
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

        public async Task<IActionResult> comboParalelos(int idGrupoCurso,int idCurso)
        {
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            var usuario = _auth.getUser();
            try
            {
                string sql = @"
                                select distinct(a.paralelo)
                                from asignacionesinstructorescalificaciones a
                                inner join gruposcursos g on g.idGrupoCurso=a.idGrupoCurso
                                inner join cursos_mallas m on m.idCurso = g.idCurso 
                                inner join cursos c on c.idCurso = m.idCursoAsociado 
                                where a.idGrupoCurso = @idGrupoCurso and a.idInstructor=@usuario and a.idCurso=@idCurso
                ";
                return Ok(await dapper.QueryAsync(sql, new { usuario, idGrupoCurso,idCurso }));
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

        public async Task<IActionResult> listar(int idGrupoCurso, int idCurso,string paralelo)
        {
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            var usuario = _auth.getUser();
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
                                and m.paralelo = a.paralelo and a.idCurso = c.idCurso and a.idInstructor = @usuario
                                where m.idGrupoCurso = @idGrupoCurso and c.idCurso=@idCurso and m.paralelo=@paralelo
                                order by e.primerApellido,e.primerNombre
                ";
                var listaCalificaciones = await dapper.QueryAsync(sql, new { usuario, idGrupoCurso, idCurso, paralelo });
                sql = @"SELECT * FROM cursos WHERE idCurso=@idCurso";
                var parametros = await dapper.QueryFirstOrDefaultAsync(sql, new { idCurso });
                return Ok(new {listaCalificaciones,parametros});
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
        public async Task<IActionResult> guardar([FromBody]calificaciones _data)
        {
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            try
            {
                string sql=@"SELECT numeroNotas,puntajeMinimo,asistenciaMinima from cursos WHERE idCurso=@idCurso";;
                var parametros=await dapper.QueryFirstOrDefaultAsync<cursos>(sql,_data);
                int? notas = parametros.numeroNotas;
                int? puntajeMinimo = parametros.puntajeMinimo;
                double acumulador=0;
                for (var i = 1; i <= notas; i++){
                   Type type=_data.GetType();
                    PropertyInfo info=type.GetProperty($"nota{i}");
                    acumulador+=info.GetValue(_data)==null?0:Convert.ToDouble(info.GetValue(_data));
                } 
                _data.promedioFinal=Convert.ToDecimal(acumulador/notas);
                _data.aprobado = Convert.ToSByte(_data.promedioFinal >= Convert.ToDecimal(puntajeMinimo) && _data.faltas<=parametros.asistenciaMinima);
                sql = @"
                                UPDATE calificaciones
                                SET nota1=@nota1, nota2=@nota2, nota3=@nota3,nota4=@nota4,nota5=@nota5,
                                faltas=@faltas,
                                promedioFinal=@promedioFinal,aprobado=@aprobado
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
            var dapper = new MySqlConnection(_context.Database.GetConnectionString());
            try
            {
                string sql = @"SELECT numeroNotas,puntajeMinimo,asistenciaMinima from cursos WHERE idCurso=@idCurso"; ;
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
                    item.aprobado = Convert.ToSByte(item.promedioFinal >= Convert.ToDecimal(puntajeMinimo) && item.faltas < parametros.asistenciaMinima);
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
