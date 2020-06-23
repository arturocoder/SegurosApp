using System.Web;
using System.Web.Optimization;

namespace ProyectoSegurosFpDaw
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {           

             //SCRIPTS DE LIBRERÍAS REQUERIDAS 
             //JQUERY /  BOOTSTRAP 4 / ADMINLTE / SWEET ALERT 2          
            //bundles.Add(new ScriptBundle("~/bundles/requiredScript")
            //    .Include("~/plugins/jquery/jquery.min.js")
            //    .Include("~/plugins/bootstrap/js/bootstrap.bundle.min.js")
            //    .Include("~/content/adminlte/dist/js/adminlte.min.js")
            //    .Include("~/plugins/sweetalert2/sweetalert2.all.min.js"));

            //CSS DE LIBRERÍAS REQUERIDAS 
            //FONT AWESOME ICONS / ADMIN LTE / SWEET ALERT 
            //SHARED CSS ( CSS PROPIOS )
            //bundles.Add(new StyleBundle("~/bundles/requiredCss")
            //    .Include(
            //          "~/plugins/fontawesome-free/css/all.min.css",                      
            //           "~/plugins/sweetalert2/sweetalert2.min.css",
            //            "~/Content/SharedCSS.css"));
        }
    }
}
