using System.Web;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Filtro que verifica la sesión
            filters.Add(new Filtros.VerificarSession());
        }
    }
}
