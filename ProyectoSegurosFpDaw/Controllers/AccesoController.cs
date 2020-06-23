using System;
using System.Linq;
using System.Web.Mvc;
using ProyectoSegurosFpDaw.Models;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class AccesoController : Controller
    {
        /// <summary>
        /// GET : Muestra formulario de Login.
        /// </summary>
        /// <returns>Vista Login</returns>
        
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary> 
        /// POST : Autenticación del Usuario mediante email / password.
        /// </summary>
        /// <param name="User">Email Usuario</param>
        /// <param name="Pass">Password Usuario</param>
        /// <returns>
        /// Ok Login => redirecciona al Index de GestiónPólizas.
        /// Error Login => devuelve mensaje de error a la vista. 
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string User, string Pass)
        {
            // Encripta la password para poder compararla con la almacenada en BBDD.
            string ePass = Encriptacion.GetSHA256(Pass);
            try
            {
                using (ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities())
                {
                    // Busca coincidencia en BBDD.                   
                    var usuarioAutenticado = db.Usuario
                        .Where(c => c.emailUsuario == User.Trim() && c.password == ePass)
                        .FirstOrDefault();
                    if (usuarioAutenticado == null)
                    {
                        ViewBag.Error = "Usuario o contraseña inválida";
                        return View();
                    }
                    // Ok autenticación => guarda en Sessión el Usuario.
                    // En 25 min, si no hay actividad de usuario, se cierra sesión
                    Session["User"] = usuarioAutenticado;
                    Session.Timeout =25;
                }
                return RedirectToAction("Index", "GestionPolizas");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        /// <summary>
        /// GET : 
        /// Abandona la sesión actual (log out).
        /// </summary>
        /// <returns>Redirecciona a la vista Login.</returns>
        [HttpGet]
        public ActionResult LoginOut()
        {
            Session["user"] = null;
            Session.Abandon();
            return RedirectToAction("Login");
        }      
    }
}