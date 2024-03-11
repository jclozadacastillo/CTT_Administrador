using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Contador;
using CTT_Administrador.Models.ctt;
using CTT_Administrador.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Contadores
{
    public class ContadoresController : Controller
    {
        private readonly IAuthorizeContadorTools _auth;
        private readonly cttContext _context;

        public ContadoresController(IAuthorizeContadorTools auth, cttContext context)
        {
            _auth = auth;
            _context = context;
        }

        public IActionResult Login()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Contadores");
            return View();
        }

        public IActionResult Index()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Contadores");
            return View();
        }

        public IActionResult ValidarPagos()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Contadores");
            return View();
        }        
        
        public IActionResult PagosValidados()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Contadores");
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
                                    where ru.idUsuario == user.idUsuario && rol.rol == "contador" && ru.activo == 1
                                    select new
                                    {
                                        rol.rol
                                    }).CountAsync();
                if (asesor == 0) throw new Exception("El usuario no tiene permisos para acceder al sistema de contabilidad");
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

        [AuthorizeContador]
        public async Task<IActionResult> validacionesPendientes()
        {
            try
            {
                var total = await (from p in _context.pagosmatriculas
                                   where p.idEstado == 0
                                   select p.idPagoMatricula).CountAsync();
                return Ok(total);
            }
            catch (Exception ex)
            {
                return Tools.handleError(ex);
            }
        }
    }
}