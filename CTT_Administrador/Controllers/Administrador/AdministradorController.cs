using CTT_Administrador.Auth;
using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTT_Administrador.Controllers.Administrador
{
    public class AdministradorController : Controller
    {
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly cttContext _context;

        public AdministradorController(IAuthorizeAdministradorTools auth, cttContext context)
        {
            _auth = auth;
            _context = context;
        }

        public IActionResult Login()
        {
            if (_auth.validateToken()) return RedirectToAction("Index", "Administrador");
            return View();
        }

        public IActionResult Index()
        {
            if (!_auth.validateToken()) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult Categorias()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult TiposCursos()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult Cursos()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult MatriculasUniandesMasivas()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult Instructores()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult Estudiantes()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult AsignacionInstructores()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult AsignacionAsistencias()
        {
            if (!_auth.inRol("admin,faltas")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult PeriodosAcademicos()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult AdministrarMatriculas()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult ListadoMatriculados()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult ModulosDiplomados()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult OfertaAcademica()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult MatriculasIndividuales()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult CreditosPendientes()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }

        public IActionResult CreditosCancelados()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }       
        
        public IActionResult ListadoPorCurso()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
            return View();
        }
        public IActionResult RecordAcademico()
        {
            if (!_auth.inRol("admin")) return RedirectToAction("Login", "Administrador");
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
                if (_context.rolesusuarios.Where(x => x.idUsuario == user.idUsuario).Count() == 0) throw new Exception("El usuario ingresado no se encuentra registrado en ningún rol");
                if (user.clave != _data.clave) throw new Exception("La contraseña ingresada no es correcta");
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