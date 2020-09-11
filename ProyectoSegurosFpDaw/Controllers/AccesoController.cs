using System;
using System.Linq;
using System.Web.Mvc;
using ProyectoSegurosFpDaw.BLL;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class AccesoController : Controller
    {
        private ProyectoSegurosDbEntities context;       
        private UsuarioBLL usuarioBll;
        
        public AccesoController()
        {
            context = new ProyectoSegurosDbEntities();
            usuarioBll = new UsuarioBLL(context);            
        }
        

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string User, string Pass)
        {
            // Encripta la password para poder compararla con la almacenada en BBDD.
            try
            {                
                var unitOfWork = new UnitOfWork(context);
                string encriptedPassword = Encriptacion.GetSHA256(Pass);
                var usuarioAutenticado = usuarioBll.GetAuthenticatedUsuario(User, encriptedPassword);

                if (usuarioAutenticado == null)
                {
                    ViewBag.Error = "Usuario o contraseña inválida";
                    return View();
                }
                
                Session["User"] = usuarioAutenticado;
                Session.Timeout = 25;
               
                return RedirectToAction("Index", "GestionPolizas");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        
        [HttpGet]
        public ActionResult LoginOut()
        {
            Session["user"] = null;
            Session.Abandon();
            return RedirectToAction("Login");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}