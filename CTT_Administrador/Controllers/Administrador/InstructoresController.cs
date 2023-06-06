using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class InstructoresController : Controller
    {
        private readonly cttContext _context;

        public InstructoresController(cttContext context)
        {
            _context = context;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> listar()
        {
            try
            {
                return Ok(await _context.instructores.OrderBy(x => x.primerApellido).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> comboTiposDocumentos()
        {
            try
            {
                return Ok(await _context.tiposdocumentos.ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> unDato(int idInstructor)
        {
            try
            {
                return Ok(await _context.instructores.FindAsync(idInstructor));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> guardar(instructores _data)
        {
            try
            {
                if (_context.instructores.Where(x => x.documentoIdentidad == _data.documentoIdentidad && _data.idInstructor != x.idInstructor).Count() > 0) throw new Exception("Lo sentimos esa documento de identidad ya se encuentra registrado");
                if (_data.idInstructor > 0)
                    _context.instructores.Update(_data);
                else _context.instructores.Add(_data);
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
        public async Task<IActionResult> activar(int idInstructor)
        {
            try
            {
                var data = await _context.instructores.FindAsync(idInstructor);
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