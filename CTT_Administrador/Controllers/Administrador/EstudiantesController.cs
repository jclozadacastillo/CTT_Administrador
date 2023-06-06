using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class EstudiantesController : Controller
    {
        private readonly cttContext _context;

        public EstudiantesController(cttContext context)
        {
            _context = context;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> listar()
        {
            try
            {
                return Ok(await _context.estudiantes.OrderBy(x => x.primerApellido).ToListAsync());
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
        public async Task<IActionResult> unDato(int idEstudiante)
        {
            try
            {
                return Ok(await _context.estudiantes.FindAsync(idEstudiante));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> guardar(estudiantes _data)
        {
            try
            {
                if (_context.estudiantes.Where(x => x.documentoIdentidad == _data.documentoIdentidad && _data.idEstudiante != x.idEstudiante).Count() > 0) throw new Exception("Lo sentimos esa documento de identidad ya se encuentra registrado");
                if (_data.idEstudiante > 0)
                    _context.estudiantes.Update(_data);
                else _context.estudiantes.Add(_data);
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
        public async Task<IActionResult> activar(int idEstudiante)
        {
            try
            {
                var data = await _context.estudiantes.FindAsync(idEstudiante);
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