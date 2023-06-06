using CTT_Administrador.Auth.Administrador;
using CTT_Administrador.Models.ctt;
using Microsoft.AspNetCore.Mvc;

namespace CTT_Administrador.Controllers.Administrador
{
    public class CreditosPendientesController : Controller
    {
        private readonly cttContext _context;
        private readonly IAuthorizeAdministradorTools _auth;
        private readonly string _path;

        public CreditosPendientesController(cttContext context, IAuthorizeAdministradorTools auth, IWebHostEnvironment _env)
        {
            _context = context;
            _auth = auth;
            _path = _env.WebRootPath;
        }
    }
}