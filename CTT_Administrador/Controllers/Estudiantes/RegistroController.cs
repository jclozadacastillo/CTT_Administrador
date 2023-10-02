using CTT_Administrador.Auth;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Estudiantes
{
    public class RegistroController : Controller
    {
        private readonly cttContext _context;
        private readonly IDbConnection _dapper;

        public RegistroController(IDbConnection db, cttContext context)
        {
            _dapper = db;
            _context = context;
        }

        public IActionResult ConfirmarCorreo()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> comboTiposDocumentos()
        {
            try
            {
                string sql = @"SELECT * FROM tiposdocumentos
                               ORDER BY ~esCedula";
                return Ok(await _dapper.QueryAsync(sql));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> registrar(estudiantes _data)
        {
            try
            {
                var estudiante = await _context.estudiantes.AsNoTracking().Where(x => x.documentoIdentidad == _data.documentoIdentidad).FirstOrDefaultAsync();
                if (estudiante != null && estudiante.confirmado != 1) throw new Exception("El usuario ya ha sido registrado con éxito pero no se ha confirmado el correo");
                if (estudiante != null && estudiante.confirmado == 1) throw new Exception("El documento de identidad ya ha sido registrado y verificado");
                _context.estudiantes.Add(_data);
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    persona = _data,
                    mailParams = new
                    {
                        idConfiguracion = ConfigurationHelper.config["correo:idConfiguracion"],
                        urlConfirm = ConfigurationHelper.config["correo:urlConfirm"],
                        urlApi = ConfigurationHelper.config["correo:urlApi"]
                    }
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> confirmarCuenta(int idEstudiante)
        {
            try
            {
                var estudiante = await _context.estudiantes.FindAsync(idEstudiante);
                if (estudiante == null) throw new Exception("No hemos encontrado registros de su cuenta, el token puede ser incorrecto o haber caducado");
                if (estudiante.confirmado == 1) return Ok(new { mensaje = "Su cuenta ya ha sido verificada" });
                string sql = @"UPDATE Estudiantes set confirmado=1,fechaConfirmacion=current_timestamp() WHERE idEstudiante=@idEstudiante";
                await _dapper.ExecuteAsync(sql, estudiante);
                return Ok(new { mensaje = "ok" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                _dapper.Dispose();
            }
        }
    }
}