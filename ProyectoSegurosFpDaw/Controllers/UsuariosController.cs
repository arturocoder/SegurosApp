using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.BLL;
using ProyectoSegurosFpDaw.Controllers.Helpers;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class UsuariosController : Controller
    {

        private ProyectoSegurosDbEntities context;
        private UnitOfWork unitOfWork;
        private UsuarioBLL usuarioBll;
        private UsuarioControllerHelper helper;
        public UsuariosController()
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
            usuarioBll = new UsuarioBLL(unitOfWork);
            helper = new UsuarioControllerHelper();
        }

        #region Actions        

             
        [HttpGet]
        [AutorizarUsuario(permisoId: 5)]
        public ActionResult Index()
        {
            // Estadosession que se envía a la vista a través de ViewBag
            // para colapsar/mostrar la sección que corresponda.
            // 1=>Buscar Usuario
            // 2=>Resultados           
            var estadoSession = "1";

            // Comprueba que haya mensajes enviado desde otra action .
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }

            // Recupera lista de usuarios coincidentes 
            // si viene de la acción BuscarUsuarios.
            if (TempData.ContainsKey("usuariosCoincidentes"))
            {
                estadoSession = "2";
                var usuariosCoincidentes = TempData["usuariosCoincidentes"];
                ViewBag.usuariosCoincidentes = usuariosCoincidentes;
            }
            var roles = unitOfWork.Rol.GetAll();
            ViewBag.rolId = helper.GetSelectListRolConOpcionTodos(roles);
            ViewBag.estadoSession = estadoSession;
            return View();
        } 
        
        [HttpGet]
        [AutorizarUsuario(permisoId: 18)]
        public ActionResult BuscarUsuarios(string nombreUsuario, string apellido1Usuario, string dniUsuario, string emailUsuario, string rolId)
        {
            try
            {
                UsuarioSearching searchingFields = usuarioBll.GetSearchingField(nombreUsuario, apellido1Usuario, dniUsuario, emailUsuario, rolId);
                List<Usuario> usuariosMatches = usuarioBll.SearchUsuarios(searchingFields);

                if (usuariosMatches.Any() && usuariosMatches.FirstOrDefault().activo == 0)
                {
                    var usuarioNoActivo = usuariosMatches.FirstOrDefault();
                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Usuario.GetNombreModelo(), usuarioNoActivo.usuarioId);
                }

                TempData["usuariosCoincidentes"] = usuariosMatches;
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionBuscar(Usuario.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [AutorizarUsuario(permisoId: 4)]
        public ActionResult Details(int id)
        {
            var usuario = unitOfWork.Usuario.GetUsuarioActivo(id);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        [HttpGet]
        [AutorizarUsuario(permisoId: 1)]
        public ActionResult Create()
        {
            ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll().OrderBy(c => c.nombreRol), "rolId", "nombreRol");
            return View();
        }

        [AutorizarUsuario(permisoId: 1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombreUsuario,apellido1Usuario,apellido2Usuario,dniUsuario,emailUsuario,password,rolId")] Usuario usuario)
        {
            if (ModelState.IsValid == false || usuarioBll.FieldsFormat(usuario) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (usuarioBll.AnyUsuarioWithDni(usuario.dniUsuario))
            {
                ViewBag.mensaje = ItemMensaje.ErrorRegistroDuplicadoCrear(Usuario.GetNombreModelo(), "NIF/NIE", null);
                ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll(), "rolId", "nombreRol");
                return View(usuario);
            }
            if (usuarioBll.AnyUsuarioWithEmail(usuario.emailUsuario))
            {
                ViewBag.mensaje = ItemMensaje.ErrorRegistroDuplicadoCrear(Usuario.GetNombreModelo(), "Email", null);
                ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll(), "rolId", "nombreRol");
                return View(usuario);
            }
            try
            {
                usuarioBll.CreateNewUsuario(usuario);
                TempData["mensaje"] = ItemMensaje.SuccessCrear(Usuario.GetNombreModelo(), usuario.dniUsuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ItemMensaje.ErrorExcepcionCrear(Usuario.GetNombreModelo(), ex.GetType().ToString());
                ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll(), "rolId", "nombreRol");
                return View(usuario);
            }
        }
      
        [HttpGet]
        [AutorizarUsuario(permisoId: 2)]
        public ActionResult Edit(int id)
        {
            var usuario = unitOfWork.Usuario.GetUsuarioActivo(id);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll(), "rolId", "nombreRol", usuario.rolId);
            return View(usuario);
        }
        
        [AutorizarUsuario(permisoId: 2)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            var usuario = unitOfWork.Usuario.GetUsuarioActivo(id);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(usuario, "", new string[] { "nombreUsuario", "apellido1Usuario", "apellido2Usuario", "emailUsuario", "password", "rolId" }) == false)
            {
                ViewBag.mensaje = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                ViewBag.rolId = new SelectList(context.Rol, "rolId", "nombreRol", usuario.rolId);
                return View(usuario);
            }
            if (usuarioBll.FieldsFormat(usuario) == false)
            {
                ViewBag.mensaje = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                ViewBag.rolId = new SelectList(context.Rol, "rolId", "nombreRol", usuario.rolId);
                return View(usuario);
            }
            if (usuarioBll.ValidateChangingRolAdministrador(usuario) == false)
            {
                ViewBag.mensaje = ItemMensaje.ErrorEditarDesactivarUnicoAdministrador(Usuario.GetNombreModelo());
                ViewBag.rolId = new SelectList(context.Rol, "rolId", "nombreRol", usuario.rolId);
                return View(usuario);
            }
            try
            {
                usuarioBll.UpdateUsuario(usuario);
                TempData["mensaje"] = ItemMensaje.SuccessEditar(Usuario.GetNombreModelo(), usuario.apellido1Usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ItemMensaje.ErrorExcepcionEditar(Usuario.GetNombreModelo(), ex.GetType().ToString());
                ViewBag.rolId = new SelectList(context.Rol, "rolId", "nombreRol", usuario.rolId);
                return View(usuario);
            }
        }

        [AutorizarUsuario(permisoId: 3)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int usuarioId)
        {
            var usuario = unitOfWork.Usuario.GetUsuarioActivo(usuarioId);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (usuarioBll.ValidateDeletingUsuarioRolAdministrador(usuario) == false)
            {
                ViewBag.mensaje = ItemMensaje.ErrorEditarDesactivarUnicoAdministrador(Usuario.GetNombreModelo());
                return View("Details", usuario);

            }
            try
            {
                usuarioBll.DeleteUsuario(usuario);
                TempData["mensaje"] = ItemMensaje.SuccessDesactivar(Usuario.GetNombreModelo(), usuario.apellido1Usuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionDesactivar(Usuario.GetNombreModelo(), ex.GetType().ToString());
                RedirectToAction("Index");
            }

            TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivar(Usuario.GetNombreModelo());
            return RedirectToAction("Index");
        }
        #endregion
        #region Métodos JSON

        /// <summary>
        /// Comprueba a través de llamada Ajax desde la vista, 
        /// si el NIF / NIE introducido existe en Usuario.  
        /// </summary>
        /// <param name="dni">NIF/NIE usuario</param>
        /// <returns>
        /// Json : 
        /// 1 => hay coincidencia.
        /// 0 => no hay coincidencia.
        /// </returns>
        [HttpGet]
        public JsonResult VerificarDniDuplicado(string dni)
        {
            if (dni.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("");
            }
            var nif = dni.Trim().ToUpperInvariant();
            Usuario usuarioCoincidente = unitOfWork.Usuario.SingleOrDefault(c => c.dniUsuario == nif);

            var respuestaJson = 1;
            if (usuarioCoincidente != null)
            {
                respuestaJson = 1;
            }
            else
            {
                respuestaJson = 0;
            }
            return Json(respuestaJson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Comprueba a través de llamada Ajax desde la vista, 
        /// si el email introducido existe en Usuario.  
        /// </summary>
        /// <param name="email">email usuario</param>
        /// <returns>
        /// Json : 
        /// 1 => hay coincidencia.
        /// 0 => no hay coincidencia.
        /// </returns>
        [HttpGet]
        public JsonResult VerificarEmailDuplicado(string email)
        {
            if (email.IsNullOrWhiteSpace())
            {
                throw new ArgumentException("");
            }
            var mail = email.Trim().ToUpperInvariant();
            Usuario usuarioCoincidente = unitOfWork.Usuario.SingleOrDefault(c => c.emailUsuario == mail);

            var respuestaJson = 1;
            if (usuarioCoincidente != null)
            {
                respuestaJson = 1;
            }
            else
            {
                respuestaJson = 0;
            }
            return Json(respuestaJson, JsonRequestBehavior.AllowGet);
        }


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
