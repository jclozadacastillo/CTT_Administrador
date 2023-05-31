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
    public class CursosController : Controller
    {
        private readonly cttContext _context;

        public CursosController(cttContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> listar()
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"SELECT c.*,tipoCurso,categoria
                            FROM cursos c
                            INNER JOIN categorias ca on ca.idCategoria=c.idCategoria
                            INNER JOIN tiposcursos t on t.idTipoCurso=c.idTipoCurso
                            WHERE c.idCurso in(select cm.idCurso
                            from cursos_mallas cm
                            where cm.idCursoAsociado=c.idCurso
                            ) or
                            c.idCurso not in(
                            select cm.idCursoAsociado
                            from cursos_mallas cm
                            )
                            ORDER BY c.fechaRegistro desc";
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

        [HttpGet]
        public async Task<IActionResult> comboCategorias()
        {
            try
            {
                return Ok(await _context.categorias.Where(x => x.activo == 1).OrderBy(x => x.categoria).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> comboTiposCursos()
        {
            try
            {
                return Ok(await _context.tiposcursos.OrderBy(x => x.tipoCurso).ToListAsync());
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
                return Ok(await _context.cursos.FindAsync(idCurso));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(cursos _data)
        {
            var dapper = new MySqlConnection(ConfigurationHelper.config.GetConnectionString("ctt"));
            try
            {
                string sql = @"DELETE FROM cursos_mallas WHERE idCurso=idCursoAsociado
                              AND idCurso=@idCurso;
                              ";
                if (_data.idCurso > 0)
                {
                    _data.fechaRegistro = await _context.cursos.AsNoTracking().Where(x => x.idCurso == _data.idCurso).Select(x => x.fechaRegistro).FirstOrDefaultAsync();
                    _data.fechaActualizacion=DateTime.Now;
                    _context.cursos.Update(_data);
                    await _context.SaveChangesAsync();
                    if (_context.tiposcursos.Where(x => x.idTipoCurso == _data.idTipoCurso).FirstOrDefault()?.esDiplomado != 1)
                    {
                        if (_context.cursos_mallas.Where(x => x.idCurso == _data.idCurso).Count() == 0)
                        {
                            _context.cursos_mallas.Add(new cursos_mallas
                            {
                                idCurso = _data.idCurso,
                                idCursoAsociado = _data.idCurso,
                                valor = _data.precioCurso,
                                activo = 1
                            });
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        await dapper.ExecuteAsync(sql, _data);
                    }
                }
                else
                {
                    _data.fechaRegistro = DateTime.Now;
                    _context.cursos.Add(_data);
                    await _context.SaveChangesAsync();
                    if (_context.tiposcursos.Where(x => x.idTipoCurso == _data.idTipoCurso).FirstOrDefault()?.esDiplomado != 1)
                    {
                        if (_context.cursos_mallas.Where(x => x.idCurso == _data.idCurso).Count() == 0)
                        {
                            _context.cursos_mallas.Add(new cursos_mallas
                            {
                                idCurso = _data.idCurso,
                                idCursoAsociado = _data.idCurso,
                                valor = _data.precioCurso,
                                activo = 1
                            });
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        await dapper.ExecuteAsync(sql, _data);
                    }
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