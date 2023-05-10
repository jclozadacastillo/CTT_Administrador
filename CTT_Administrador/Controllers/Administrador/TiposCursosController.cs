using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class TiposCursosController : Controller
    {
        private readonly cttContext _context;

        public TiposCursosController(cttContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> listar()
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
        public async Task<IActionResult> unDato(int idTipoCurso)
        {
            try
            {
                return Ok(await _context.tiposcursos.FindAsync(idTipoCurso));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(tiposcursos _data)
        {
            try
            {
                if (_data.idTipoCurso > 0)
                    _context.tiposcursos.Update(_data);
                else _context.tiposcursos.Add(_data);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> activar(int idTipoCurso)
        {
            try
            {
                var data = await _context.tiposcursos.FindAsync(idTipoCurso);
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