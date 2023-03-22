﻿using CTT_Administrador.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CTT_Administrador.Controllers
{
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}