using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;

namespace ProyectoSegurosFpDaw.Controllers  
{
    [RequireHttps]
    public class RolesPermisosController : Controller
    {
        private ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities();

        #region Actions

        /// <summary>
        /// GET :  Muestra lista de Roles / Permisos.
        /// </summary>
        /// <returns>Vista con la lista de roles/permisos</returns>
        [AutorizarUsuario(permisoId: 11)]
        [HttpGet]
        public ActionResult Index()
        {
            // Envía una lista de roles ,con los roles permisos. 
            var roles = db.Rol.Include(r => r.RolPermiso).ToList();                        
            return View(roles);        
        }
        #endregion

        #region Métodos
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
