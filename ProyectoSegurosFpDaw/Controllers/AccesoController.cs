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
        private UnitOfWork unitOfWork;
        private AccesoBLL accessoBLL; 
        
        public AccesoController() 
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
            accessoBLL = new AccesoBLL(unitOfWork);            
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
            try
            {                
                var unitOfWork = new UnitOfWork(context);                
                var usuarioAutenticado = accessoBLL.GetAuthenticatedUsuario(User, Pass);

                if (usuarioAutenticado == null)
                {
                    ViewBag.Error = "Usuario o contraseña inválida";
                    return View();
                }
                
                Session["User"] = usuarioAutenticado;
                Session.Timeout = 25;
               
                return RedirectToAction("Index", "GestionPolizas");
            }
            catch (Exception)
            {
                ViewBag.Error = "Error inesperado al intentar autenticar al usuario";
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