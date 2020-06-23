using ProyectoSegurosFpDaw.Controllers;
using ProyectoSegurosFpDaw.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoSegurosFpDaw.Filtros
{
    //filtro para vericar que la sesión es valida
    public class VerificarSession:ActionFilterAttribute
    {
        private Usuario oUsuario;
        //se sobreescribe el método
        //Es llamado antes que se ejecute la acción
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                //guarda el usuario que existe en la Session ,realizando un casting 
                oUsuario = (Usuario)HttpContext.Current.Session["User"];
                //Si no hay Session,devuelve a la página de Login
                if (oUsuario == null)
                {
                    //Si se intenta acceder al controlador home, lo permite
                    if(filterContext.Controller is HomeController)
                    {
                        return;
                    }
                    //si se intenta acceder a cualquier controlador diferente a login, 
                    //redirige a login.
                    if(filterContext.Controller is AccesoController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("~/Acceso/Login");
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}