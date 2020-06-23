using ProyectoSegurosFpDaw.Models;
using System;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult About()
        {       
            return View();
        }
        [HttpGet]
        public ActionResult Reiniciar()
        {
            return View();
        }

        /// <summary>
        /// Reinicia los registros de la base de datos
        /// a los datos originales. 
        /// NOTA : Este Action está enfocado al proceso de evaluación del proyecto desplegado.
        /// Se ha implementado para que cuando el profesor esté evaluando el proyecto,
        /// si por cualquier motivo lo necesita (p.e password modificada no recordada,datos corruptos...),
        /// pueda reiniciar los datos de la BBDD a los originales.
        /// En un entorno de producción real se eliminaría por motivos de seguridad.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reiniciar(string user, string Pass)
        {
            string eUser = "ONLINE2020ILERNA";
            string ePass = "DAW3333pgs";           
            try
            {
                if (Pass != null)
                {
                    if (Pass.Equals(ePass) && user.Equals(eUser))
                    {
                        if (ReinicioBBDD.ReiniciarBBDD() == true)
                        {
                            ViewBag.msj = "Proceso de reinicio BBDD correcto. ";
                        }
                        else
                        {
                            ViewBag.msj = "Ha ocurrido un error en el proceso de reinicio de la BBDD.";

                        }

                        return View();
                    }
                    else
                    {
                        ViewBag.msj = "Usuario o Password incorrecta";
                        return View();

                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.msj= "Ha ocurrido un error en el proceso de reinicio de la BBDD. Excepción :  "+ ex.GetType();
                return View();
            }
            ViewBag.msj = "Ha ocurrido un error en el proceso de reinicio de la BBDD.";
            return View();
        }

    }
}