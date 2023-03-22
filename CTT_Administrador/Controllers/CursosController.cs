using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace CTT_Administrador.Controllers
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
            var dapper=new MySqlConnection(_context.Database.GetConnectionString());
            try
            {
                string sql=@"SELECT c.*,tipoCurso,categoria 
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
                            ORDER BY c.curso";
                return Ok(await dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }finally{
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
            try
            {
                if (_data.idCurso > 0)
                    _context.cursos.Update(_data);
                else _context.cursos.Add(_data);
                await _context.SaveChangesAsync();
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