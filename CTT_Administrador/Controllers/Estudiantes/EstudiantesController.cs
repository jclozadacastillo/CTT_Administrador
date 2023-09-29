using CTT_Administrador.Models.ctt;
using CTT_Estudiante.Auth.Estudiante;
using Microsoft.AspNetCore.Mvc;
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
	}
}