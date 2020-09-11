using ProyectoSegurosFpDaw.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Controllers.Helpers
{
    public class UsuarioControllerHelper
    {        
        /// <returns> List SelectListItem con lista de roles 
        ///  y la opción con valor 0 y texto Todo</returns>
        public List<SelectListItem> GetSelectListRolConOpcionTodos(IEnumerable<Rol> roles)
        {           
            var rolesSelectList = new List<SelectListItem>();
            rolesSelectList.Add(new SelectListItem { Value = "0", Text = "TODOS" });            

            foreach (var item in roles.OrderBy(c=>c.nombreRol))
            {
                rolesSelectList.Add(new SelectListItem { Value = item.rolId.ToString(CultureInfo.GetCultureInfo("es-ES")), Text = item.nombreRol });
            }
            return rolesSelectList;
        }
    }
}