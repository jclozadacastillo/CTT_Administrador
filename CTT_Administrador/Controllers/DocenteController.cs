using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Docente;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers
{
    public class DocenteController : Controller
    {
        private readonly IAuthorizeDocenteTools _auth;
        private readonly cttContext _context;

        public DocenteController(IAuthorizeDocenteTools auth, cttContext context)
        {
            _auth = auth;
            _context = context;
        }

        public IActionResult Login()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Docente");
            return View();
        }

        public IActionResult Index()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Docente");
            return View();
        }

        public IActionResult Calificaciones()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Docente");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> authorization(TokenTools.userData _data)
        {
            try
            {
                var user = await _context.instructores.Where(x => x.documentoIdentidad == _data.usuario).FirstOrDefaultAsync();
                if (user == null) throw new Exception("El usuario ingresado no se encuentra registrado");
                if (Convert.ToBoolean(user.activo) != true) throw new Exception("El usuario ingresado no se encuentra activo");
                if (user.elPassword != _data.clave) throw new Exception("La contraseña ingresada no es correcta");
                _data.nombre = $"{user.abreviaturaTitulo?.Replace(".", "")}. {user.primerApellido} {user.segundoApellido} {user.primerNombre} {user.segundoNombre}";
                _data.usuario = Convert.ToString(user.idInstructor);
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
    }
}