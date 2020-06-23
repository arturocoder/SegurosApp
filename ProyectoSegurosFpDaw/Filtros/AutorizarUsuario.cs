using ProyectoSegurosFpDaw.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Filtros
{
    // Filtro para autorizar el uso de los métodos elegidos al  usuario
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AutorizarUsuarioAttribute : AuthorizeAttribute
    {
        private Usuario oUsuario;
        private int permisoId;

        public AutorizarUsuarioAttribute(int permisoId = 0)
        {
            this.permisoId = permisoId;
        }
        // Sobreescritura del método 
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Nombre operación (crear,editar,eliminar..)
            string nombrePermiso = "";
            // Nombre modulo(Póliza, Rol, Usuario...)
            string nombreModulo = "";
            try
            {
                // Guarda el usuario de la sessión.
                oUsuario = (Usuario)HttpContext.Current.Session["User"];
                // Instancia de la BBDD.
                using (ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities())
                {
                    // Comprueba dentro de rolPermisos que el rol del usuario de la session , tenga el id de la operación que le pasamos como parámetro.
                    var lstMisPermisos = from m in db.RolPermiso
                                         where m.rolId == oUsuario.rolId && m.permisoId == permisoId
                                         select m;

                    // Si no tiene permiso, devuelve vista de error, con mensaje
                    if (lstMisPermisos.ToList().Count == 0)
                    {
                        var oPermiso = db.Permiso.Find(permisoId);
                        int? idModulo = oPermiso.moduloId;
                        nombrePermiso = getNombreDePermiso(permisoId);
                        nombreModulo = getNombreDeModulo(idModulo);
                        nombreModulo = nombreModulo.Replace(" ", "+");
                        nombrePermiso = nombrePermiso.Replace(" ", "+");

                        filterContext.Result = new RedirectResult("~/Error/OperacionNoAutorizada?operacion=" + nombrePermiso + "&&modulo=" + nombreModulo + "&&msjErrorExcepcion=AccesoRestringido");
                    }
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~/Error/OperacionNoAutorizada?operacion=" + nombrePermiso + "&&modulo=" + nombreModulo + "&&msjErrorExcepcion=" + ex.Message);
            }


        }

        public string getNombreDePermiso(int idPermiso)
        {
            using (ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities())
            {
                var perm = from op in db.Permiso
                           where op.permisoId == idPermiso
                           select op.nombrePermiso;
                string nombrePermiso;
                try
                {
                    nombrePermiso = perm.First();
                }
                catch (Exception)
                {
                    nombrePermiso = "";

                }
                return nombrePermiso;

            }

        }
        public string getNombreDeModulo(int? idModulo)
        {
            using (ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities())
            {
                var modulo = from m in db.Modulo
                             where m.moduloId == idModulo
                             select m.nombreModulo;
                string nombreModulo;
                try
                {
                    nombreModulo = modulo.First();
                }
                catch (Exception)
                {
                    nombreModulo = "";

                }
                return nombreModulo;
            }
        }
    }
}