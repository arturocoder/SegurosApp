using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class RolesPermisosController : Controller
    {
        private ProyectoSegurosDbEntities context;
        private UnitOfWork unitOfWork;
        public RolesPermisosController()
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
        }
        #region Actions
              
        [AutorizarUsuario(permisoId: 11)]
        [HttpGet]
        public ActionResult Index()
        {            
            var roles = unitOfWork.Rol.GetRolesWithRolesPermisos();
            return View(roles);
        }
        #endregion

        #region Métodos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
