using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Controllers.Helpers
{
    public class GestionPolizasControllerHelper
    {
        /// <summary>
        /// Crea una lista SelectListItem con estados de póliza, con 3 valores / textos 
        /// <para>0 => no vigente</para>
        /// <para>1 => vigente</para>
        /// <para>2 => todos</para>
        /// </summary>
        /// <returns> retorna un List SelectListItem </returns>                    
        public List<SelectListItem> GetSelectListEstadoPolizaConOpcionTodos()
        {
            var estadoPolizasSelectList = new List<SelectListItem>();
            estadoPolizasSelectList.Add(new SelectListItem { Value = "2", Text = "TODOS" });
            estadoPolizasSelectList.Add(new SelectListItem { Value = "1", Text = "VIGENTE" });
            estadoPolizasSelectList.Add(new SelectListItem { Value = "0", Text = "NO VIGENTE" });
            return estadoPolizasSelectList;
        }
    }
}