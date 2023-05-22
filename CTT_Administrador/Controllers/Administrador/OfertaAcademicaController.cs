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
    public class OfertaAcademicaController : Controller
    {
        private readonly cttContext _context;

        public OfertaAcademicaController(cttContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> listar(int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"select g.*,c.curso,t.tipoCurso
                                from gruposcursos g
                                inner join cursos c on c.idCurso = g.idCurso
                                inner join tiposcursos t on t.idTipoCurso = c.idTipoCurso
                                where g.idPeriodo = @idPeriodo and c.activo=1";
                return Ok(await dapper.QueryAsync(sql, new { idPeriodo }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper?.Dispose();
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboPeriodos()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT * FROM Periodos
                              order by activo
                            ";

                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper?.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> comboCursos(int idPeriodo)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT c.*,t.tipoCurso
                                FROM cursos c
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
                                and c.activo =1 and t.activo=1
                                and c.idCurso not in(SELECT idCurso FROM gruposcursos
                                WHERE idPeriodo=@idPeriodo)
                            ";

                return Ok(await dapper.QueryAsync(sql, new { idPeriodo }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper?.Dispose();
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboModalidades()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT * FROM Modalidades
                            ";

                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                dapper?.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> unDato(int idGrupoCurso)
        {
            try
            {
                var grupoCurso = await _context.gruposcursos.FindAsync(idGrupoCurso);
                var curso = await _context.cursos.FindAsync(grupoCurso?.idCurso);
                return Ok(new { curso, grupoCurso });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(gruposcursos _data)
        {
            try
            {
                _data.esVisible = 1;
                if (_data.idGrupoCurso > 0)
                    _context.gruposcursos.Update(_data);
                else _context.gruposcursos.Add(_data);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> activar(int idGrupoCurso)
        {
            try
            {
                var data = await _context.gruposcursos.FindAsync(idGrupoCurso);
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