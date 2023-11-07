using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Estudiante;
using CTT_Administrador.Models.ctt;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CTT_Administrador.Controllers.Estudiantes
{
    public class EstudiantesController : Controller
    {
        private readonly IAuthorizeEstudianteTools _auth;
        private readonly IDbConnection _dapper;
        private readonly cttContext _context;

        public EstudiantesController(IAuthorizeEstudianteTools auth, IDbConnection db, cttContext context)
        {
            _auth = auth;
            _dapper = db;
            _context = context;
        }

        public IActionResult Login()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Estudiantes");
            return View();
        }

        public IActionResult Registro()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Estudiantes");
            return View();
        }

        public IActionResult Index()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Estudiantes");
            return View();
        }

        public IActionResult Matriculas()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Estudiantes");
            return View();
        }

        public IActionResult Calificaciones()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Estudiantes");
            return View();
        }

        public IActionResult Perfil()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Estudiantes");
            return View();
        }

        public IActionResult RecuperarCuenta()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Estudiantes");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> authorization(TokenTools.userData _data)
        {
            try
            {
                var user = await _context.estudiantes.Where(x => x.documentoIdentidad == _data.usuario).FirstOrDefaultAsync();
                if (user == null) throw new Exception("El documento ingresado ingresado no se encuentra registrado en el sistema.");
                if (user.confirmado != 1) throw new Exception("Aún no se ha verificado el correo electrónico");
                if (user.clave != _data.clave) throw new Exception("La contraseña ingresada no es correcta");
                _data.nombre = $"{user.primerApellido} {user.primerNombre}";
                _data.usuario = user.idEstudiante.ToString();
                _data.email = user.email;
                string token = _auth.login(_data);
                if (string.IsNullOrEmpty(token)) throw new Exception("A ocurrido un error al iniciar sesión por favor vuelva a intentarlo");
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> logout()
        {
            await _auth.logoutAsync();
            return Ok();
        }

        [AuthorizeEstudiante]
        [HttpGet]
        public async Task<IActionResult> datosPersonales()
        {
            try
            {
                string sql = @"SELECT idEstudiante, idTipoDocumento,
                            documentoIdentidad, primerApellido, segundoApellido, primerNombre,
                            segundoNombre, direccion, celular, email, observacion, sexo, activo
                            FROM estudiantes
                            WHERE idEstudiante=@idEstudiante;";
                return Ok(await _dapper.QueryFirstOrDefaultAsync(sql, new { idEstudiante = _auth.getUser() }));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeEstudiante]
        [HttpPost]
        public async Task<IActionResult> actualizarDatos(estudiantes _data)
        {
            try
            {
                _data.idEstudiante = Convert.ToInt32(_auth.getUser());
                string sql = @"UPDATE estudiantes
                            SET direccion=@direccion,
                            celular=@celular
                            WHERE idEstudiante=@idEstudiante;";
                await _dapper.ExecuteAsync(sql, _data);
                _data.idEstudiante = Convert.ToInt32(_auth.getUser());
                var estudiante = await _context.estudiantes.AsNoTracking().Where(x => x.idEstudiante == _data.idEstudiante).FirstOrDefaultAsync();
                if (_data.email != estudiante?.email)
                {
                    _data.fechaConfirmacion = DateTime.Now;
                    sql = @"UPDATE estudiantes
                            SET email=@email,
                            confirmado=1,fechaConfirmacion=@fechaConfirmacion
                            WHERE idEstudiante=@idEstudiante;";
                    await _dapper.ExecuteAsync(sql, _data);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeEstudiante]
        [HttpPost]
        public async Task<IActionResult> validarClave(string claveAnterior)
        {
            try
            {
                var idEstudiante = Convert.ToInt32(_auth.getUser());
                var estudiante = await _context.estudiantes.AsNoTracking().Where(x => x.idEstudiante == idEstudiante && claveAnterior == x.clave).FirstOrDefaultAsync();
                if (estudiante == null) throw new Exception("La contraseña ingresada no es correcta");
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [AuthorizeEstudiante]
        [HttpPost]
        public async Task<IActionResult> actualizarClave(estudiantes _data)
        {
            try
            {
                _data.idEstudiante = Convert.ToInt32(_auth.getUser());
                string sql = @"UPDATE estudiantes
                            SET clave=@clave
                            WHERE idEstudiante=@idEstudiante;";
                await _dapper.ExecuteAsync(sql, _data);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}