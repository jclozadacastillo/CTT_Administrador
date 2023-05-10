using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class CategoriasController : Controller
    {
        private readonly cttContext _context;

        public CategoriasController(cttContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> listar()
        {
            try
            {
                return Ok(await _context.categorias.OrderBy(x => x.categoria).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> unDato(int idCategoria)
        {
            try
            {
                return Ok(await _context.categorias.FindAsync(idCategoria));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> guardar(categorias _data)
        {
            try
            {
                if (_data.idCategoria > 0)
                    _context.categorias.Update(_data);
                else _context.categorias.Add(_data);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> activar(int idCategoria)
        {
            try
            {
                var data = await _context.categorias.FindAsync(idCategoria);
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