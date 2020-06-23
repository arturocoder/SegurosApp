using System;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class ErrorController : Controller
    {
        /// <summary>
        /// GET : 
        /// Ejecutado cuando el usuario intenta acceder a un Action / Controller no autorizado.
        /// </summary>
        /// <param name="operacion">operación/action/método</param>
        /// <param name="modulo">controlador</param>
        /// <param name="msjErrorExcepcion">mensaje de Error</param>
        /// <returns>Vista OperaciónNoAutorizada con mensaje de error.</returns>
        [HttpGet]
        public ActionResult OperacionNoAutorizada(String operacion,String modulo,String msjErrorExcepcion)
        {
            ViewBag.operacion = operacion;
            ViewBag.modulo = modulo;
            ViewBag.msjErrorExc = msjErrorExcepcion;
            return View();
        }
    }
}