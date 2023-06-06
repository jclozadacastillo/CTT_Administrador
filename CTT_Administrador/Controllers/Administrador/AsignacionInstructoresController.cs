using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class AsignacionInstructoresController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;

        public AsignacionInstructoresController(cttContext context, IAuthorizeAdministradorTools auth)
        {
            _context = context;
            _auth = auth;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> listar()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT a.*,i.documentoIdentidad,i.abreviaturaTitulo,
                              i.primerApellido,i.segundoApellido,i.primerNombre,i.segundoNombre,c.curso,detalle,
                              DATE_FORMAT(a.fechaRegistro,'%d-%m-%Y') as fechaRegistroMostrar
                              FROM asignacionesinstructorescalificaciones a
                              INNER JOIN GruposCursos g on g.idGrupoCurso = a.idGrupoCurso
                              INNER JOIN Cursos c on c.idCurso=a.idCurso
                              INNER JOIN Instructores i on i.idInstructor=a.idInstructor
                              INNER JOIN Periodos p on p.idPeriodo=g.idPeriodo
                              ORDER BY fechaRegistro DESC
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

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> comboCursosAsociados(int idGrupoCurso)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"select m.idCursoAsociado,c.curso
                                from gruposcursos g
                                inner join cursos_mallas m on m.idCurso = g.idCurso
                                inner join cursos c on c.idCurso = m.idCursoAsociado
                                where idGrupoCurso = @idGrupoCurso
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
        [HttpGet]
        public async Task<IActionResult> comboInstructores()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                                SELECT * FROM Instructores
                                WHERE activo=1
                                ORDER BY primerApellido, primerNombre DESC
                              ";
                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> unDato(int idAsignacion)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT a.*,i.documentoIdentidad,i.abreviaturaTitulo,
                              i.primerApellido,i.segundoApellido,i.primerNombre,i.segundoNombre,c.curso,detalle,
                              g.idPeriodo,c.idTipoCurso,cm.idCurso as idCursoPadre
                              FROM asignacionesinstructorescalificaciones a
                              INNER JOIN GruposCursos g on g.idGrupoCurso = a.idGrupoCurso
                              INNER JOIN Cursos c on c.idCurso=a.idCurso
                              INNER JOIN cursos_mallas cm on cm.idCurso=g.idCurso and cm.idCursoAsociado = a.idCurso
                              INNER JOIN Instructores i on i.idInstructor=a.idInstructor
                              INNER JOIN Periodos p on p.idPeriodo=g.idPeriodo
                              WHERE idAsignacion=@idAsignacion";
                return Ok(await dapper.QueryFirstOrDefaultAsync(sql, new { idAsignacion }));
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
        public async Task<IActionResult> guardar(asignacionesinstructorescalificaciones _data)
        {
            try
            {
                //_data.idCurso = await _context.gruposcursos.Where(x => x.idGrupoCurso == _data.idGrupoCurso).Select(x => x.idCurso).FirstOrDefaultAsync();
                _data.usuarioRegistra = _auth.getUser();
                _data.fechaRegistro = DateTime.Now;
                if (_data.idAsignacion > 0)
                {
                    _context.asignacionesinstructorescalificaciones.Update(_data);
                }
                else
                {
                    if (_context.asignacionesinstructorescalificaciones.Where(x => x.idCurso == _data.idCurso && x.paralelo == _data.paralelo && x.activo == 1).Count() > 0) throw new Exception("Ya existe un instructor asignado para ese curso en ese paralelo");
                    _context.asignacionesinstructorescalificaciones.Add(_data);
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> activar(int idAsignacion)
        {
            try
            {
                var data = await _context.asignacionesinstructorescalificaciones.FindAsync(idAsignacion);
                if (data == null) throw new Exception("Elemento no encontrado");
                data.activo = Convert.ToBoolean(data.activo) == true ? Convert.ToSByte(false) : Convert.ToSByte(true);
                _context.Update(data);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}