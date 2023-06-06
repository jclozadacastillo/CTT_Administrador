using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    [AuthorizeAdministrador]
    public class PeriodosAcademicosController : Controller
    {
        private readonly cttContext _context;

        public PeriodosAcademicosController(cttContext context)
        {
            _context = context;
        }

        [AuthorizeAdministrador]
        [HttpGet]
        public async Task<IActionResult> listar()
        {
            try
            {
                return Ok(await _context.periodos.OrderByDescending(x => x.fechaFin).ToListAsync());
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> unDato(int idPeriodo)
        {
            try
            {
                return Ok(await _context.periodos.FindAsync(idPeriodo));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeAdministrador]
        [HttpPost]
        public async Task<IActionResult> guardar(periodos _data)
        {
            try
            {
                if (_data.idPeriodo > 0)
                    _context.periodos.Update(_data);
                else _context.periodos.Add(_data);
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
        public async Task<IActionResult> activar(int idPeriodo)
        {
            try
            {
                var data = await _context.periodos.FindAsync(idPeriodo);
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