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

        public  IActionResult ConfirmarCorreo()
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
    }
}