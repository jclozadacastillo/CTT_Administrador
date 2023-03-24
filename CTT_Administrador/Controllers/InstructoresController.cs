using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Chart;
using Rotativa.AspNetCore;

namespace CTT_Administrador.Controllers
{
    [AuthorizeAdministrador]
    public class InstructoresController : Controller
    {
        private readonly cttContext _context;

        public InstructoresController(cttContext context)
        {
            _context = context;
        }

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
        [HttpGet]
        public async Task<IActionResult> comboTiposDocumentos()
        {
            try
            {
                return Ok(await _context.tiposdocumentos.ToListAsync());
            }catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
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

        [AllowAnonymous]
        public IActionResult pdfDemo()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult pdf()
        {
            var datos = new
            {
                nombre = "Juan Carlos",
                apellido = "Lozada Castillo",
                lista = new List<dynamic>()
                {
                 new { datoLista = "1" },
                 new { datoLista = "2" },
                 new { datoLista = "3" }
                }
            };
            return new ViewAsPdf("pdfDemo", datos);
        }
    }
}