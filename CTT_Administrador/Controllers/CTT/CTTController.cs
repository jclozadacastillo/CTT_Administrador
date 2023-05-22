using Microsoft.AspNetCore.Mvc;

namespace CTT_Administrador.Controllers.CTT
{
    public class CTTController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}