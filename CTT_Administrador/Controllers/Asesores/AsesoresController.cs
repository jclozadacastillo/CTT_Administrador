using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Asesor;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Asesores
{
    public class AsesoresController : Controller
    {
        private readonly IAuthorizeAsesorTools _auth;
        private readonly cttContext _context;

        public AsesoresController(IAuthorizeAsesorTools auth, cttContext context)
        {
            _auth = auth;
            _context = context;
        }

        public IActionResult Login()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Asesores");
            return View();
        }

        public IActionResult Index()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }

        public IActionResult Estudiantes()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }
        public IActionResult Clientes()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }

        public IActionResult MatriculasIndividuales()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }

        public IActionResult MatriculasInHouse()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }
        public IActionResult Recaudacion()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }
        public IActionResult RecaudacionInHouse()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Asesores");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> authorization(TokenTools.userData _data)
        {
            try
            {
                var user = await _context.usuarios.Where(x => x.usuario == _data.usuario).FirstOrDefaultAsync();
                if (user == null) throw new Exception("El usuario ingresado no se encuentra registrado");
                if (Convert.ToBoolean(user.activo) != true) throw new Exception("El usuario ingresado no se encuentra activo");
                if (user.clave != _data.clave) throw new Exception("La contraseña ingresada no es correcta");
                var asesor = await (from rol in _context.roles
                                    join ru in _context.rolesusuarios on rol.idRol equals ru.idRol
                                    where ru.idUsuario == user.idUsuario && rol.rol == "asesor" && ru.activo == 1
                                    select new
                                    {
                                        rol.rol
                                    }).CountAsync();
                if (asesor == 0) throw new Exception("El usuario no tiene permisos para acceder al sistema de asesores");
                _data.nombre = user.nombre;
                _data.usuario = user.usuario;
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