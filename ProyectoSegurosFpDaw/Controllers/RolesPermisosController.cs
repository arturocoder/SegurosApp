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
        private ProyectoSegurosDbEntities context = new ProyectoSegurosDbEntities();

        #region Actions

        /// <summary>
        /// GET :  Muestra lista de Roles / Permisos.
        /// </summary>
        /// <returns>Vista con la lista de roles/permisos</returns>
        [AutorizarUsuario(permisoId: 11)]
        [HttpGet]
        public ActionResult Index()
        {            
            var unitOfWork = new UnitOfWork(context);
            var roles = unitOfWork.Roles.GetRolesWithRolesPermisos();
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
