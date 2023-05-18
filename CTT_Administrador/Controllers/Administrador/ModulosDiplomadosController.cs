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
    public class ModulosDiplomadosController : Controller
    {
        private readonly cttContext _context;

        public ModulosDiplomadosController(cttContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> listar(int idCursoDiplomado)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"select c.* from cursos_mallas cm
                            inner join cursos c on c.idCurso = cm.idCursoAsociado
                            where cm.idCurso = @idCursoDiplomado";
                return Ok(await dapper.QueryAsync(sql, new { idCursoDiplomado }));
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
        public async Task<IActionResult> comboDiplomados()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"
                            select * from cursos
                            where idTipoCurso in(
                            select idTipoCurso from tiposcursos
                            where esDiplomado=1
                            )and idCurso not in(
                            select idCursoAsociado from cursos_mallas
                            ) and activo=1
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
        public async Task<IActionResult> datosDiplomado(int idCursoDiplomado)
        {
            try
            {
                var cursoPadre = await _context.cursos.FindAsync(idCursoDiplomado);
                return Ok(cursoPadre);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> unDato(int idCurso)
        {
            try
            {
                var curso = await _context.cursos.FindAsync(idCurso);
                return Ok(curso);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(cursos _data, int idCursoDiplomado)
        {
            try
            {
                var cursoPadre = await _context.cursos.FindAsync(idCursoDiplomado);
                _data.idTipoCurso = cursoPadre.idTipoCurso;
                _data.idCategoria = cursoPadre.idCategoria;
                if (_data.idCurso > 0)
                {
                    _context.cursos.Update(_data);
                    await _context.SaveChangesAsync();
                    var curso_malla = await _context.cursos_mallas.Where(x => x.idCurso == idCursoDiplomado && x.idCursoAsociado == _data.idCurso).FirstOrDefaultAsync();
                    if (curso_malla != null)
                    {
                        curso_malla.valor = _data.precioCurso;
                        _context.cursos_mallas.Update(curso_malla);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    _context.cursos.Add(_data);
                    await _context.SaveChangesAsync();
                        _context.cursos_mallas.Add(new cursos_mallas
                        {
                            idCurso = idCursoDiplomado,
                            idCursoAsociado = _data.idCurso,
                            valor = _data.precioCurso,
                            activo = 1
                        });
                        await _context.SaveChangesAsync();
                    
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> activar(int idCurso)
        {
            try
            {
                var data = await _context.cursos.FindAsync(idCurso);
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