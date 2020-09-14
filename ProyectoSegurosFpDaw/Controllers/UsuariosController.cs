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

        /// <summary>
        /// GET : muestra formulario para buscar Usuarios y listado con los resultados.
        /// </summary>
        /// <returns>Vista con formulario y resultados.</returns>       
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
        /// <summary>
        /// GET : busca en la BBDD Clientes que coincidan con los parámetros introducidos.
        /// </summary>
        /// <param name="nombreUsuario">nombre Usuario</param>
        /// <param name="apellido1Usuario">1er Apellido Usuario</param>
        /// <param name="dniUsuario">NIF/NIE Usuario</param>
        /// <param name="emailUsuario">email Usuario</param>
        /// <param name="rolId">rol Id Usuario</param>
        /// <returns>
        /// Hay coincidencias de usuarios activos => envía una lista Usuario de coincidencias y redirecciona al Index para mostrarlos.
        /// Hay coincidencias de usuario no activos => envía un mensaje de información y redirecciona al Index.
        /// Sin coincidencias => envía una lista Usuario vacía y redirecciona a Index .
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [HttpGet]
        [AutorizarUsuario(permisoId: 18)]
        public ActionResult BuscarUsuarios(string nombreUsuario, string apellido1Usuario, string dniUsuario, string emailUsuario, string rolId)
        {
            // Validaciones y formato de parámetros.      
            int rolID = 0;
            if (rolId.IsNullOrWhiteSpace() == false)
            {
                bool success = Int32.TryParse(rolId, out rolID);
                if (success == false)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosBuscar(Usuario.GetNombreModelo());
                    return RedirectToAction("Index");
                }
            }
            if (nombreUsuario.IsNullOrWhiteSpace() == false) { nombreUsuario = nombreUsuario.Trim().ToUpperInvariant(); }
            if (apellido1Usuario.IsNullOrWhiteSpace() == false) { apellido1Usuario = apellido1Usuario.Trim().ToUpperInvariant(); }
            if (dniUsuario.IsNullOrWhiteSpace() == false) { dniUsuario = dniUsuario.Trim().ToUpperInvariant(); }
            if (emailUsuario.IsNullOrWhiteSpace() == false) { emailUsuario = emailUsuario.Trim().ToUpperInvariant(); }

            // Búsqueda por parámetros
            // Filtra por los parámetros que no están vacíos , 
            // busca coincidencias en la BBDD, 
            // envía la lista de usuarios activos (activo==1) coincidentes a la acción Index.
            // rolId == 0 => Todos los roles.

            try
            {
                // Rol (resto de campos vacíos).
                if (nombreUsuario.Length == 0 && apellido1Usuario.Length == 0 && dniUsuario.Length == 0 && emailUsuario.Length == 0)
                {
                    // Todos los roles == todos los usuarios activos.
                    if (rolId == "0")
                    {
                        TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRoles();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.rolId == rolID);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Nombre y 1er Apellido + Rol.
                    if (nombreUsuario.Length > 0 && apellido1Usuario.Length > 0)
                    {
                        // Todos los roles
                        if (rolId == "0")
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombreUsuario && c.apellido1Usuario == apellido1Usuario);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombreUsuario && c.apellido1Usuario == apellido1Usuario && c.rolId == rolID);
                            return RedirectToAction("Index");
                        }
                    }
                    // Nombre + Rol.
                    else if (nombreUsuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombreUsuario);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombreUsuario && c.rolId == rolID);
                            return RedirectToAction("Index");
                        }
                    }
                    // Apellido + Rol.
                    else if (apellido1Usuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.apellido1Usuario == apellido1Usuario);
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.apellido1Usuario == apellido1Usuario && c.rolId == rolID);
                            return RedirectToAction("Index");
                        }
                    }
                    // NIF/NIE.
                    else if (dniUsuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            var usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.dniUsuario == dniUsuario);

                            // Si no hay coincidencia en cliente activo, busca en clientes no activos,
                            // y envía mensaje si hay coincidencia.
                            if (usuariosCoincidentes.Any() == false)
                            {
                                var usuarioNoActivo = unitOfWork.Usuario.GetUsuariosNoActivosWithRolesWhere(c => c.dniUsuario == dniUsuario).FirstOrDefault();
                                if (usuarioNoActivo != null)
                                {
                                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Usuario.GetNombreModelo(), usuarioNoActivo.usuarioId);
                                }
                            }
                            TempData["usuariosCoincidentes"] = usuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.dniUsuario == dniUsuario && c.rolId == rolID);
                            return RedirectToAction("Index");
                        }
                    }
                    // Email.
                    else if (emailUsuario.Length > 0)
                    {
                        //Todos los roles.
                        if (rolId == "0")
                        {
                            var usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.emailUsuario == emailUsuario);
                            if (usuariosCoincidentes.Any() == false)
                            {
                                var usuarioNoActivo = unitOfWork.Usuario.GetUsuariosNoActivosWithRolesWhere(c => c.emailUsuario == emailUsuario).FirstOrDefault();
                                if (usuarioNoActivo != null)
                                {
                                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Usuario.GetNombreModelo(), usuarioNoActivo.usuarioId);
                                }
                            }
                            TempData["usuariosCoincidentes"] = usuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                        else
                        {

                            TempData["usuariosCoincidentes"] = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.emailUsuario == emailUsuario && c.rolId == rolID);
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionBuscar(Usuario.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [AutorizarUsuario(permisoId: 4)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            var usuario = unitOfWork.Usuario.GetUsuarioActivoWhere(c => c.usuarioId == id);
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

        /// <summary>
        /// GET: formulario para editar un usuario.
        /// </summary>
        /// <param name="id">usuario Id</param>
        /// <returns>Vista con formulario para editar un usuario</returns>
        [HttpGet]
        [AutorizarUsuario(permisoId: 2)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            var usuario = unitOfWork.Usuario.GetUsuarioActivoWhere(c => c.usuarioId == id);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            ViewBag.rolId = new SelectList(unitOfWork.Rol.GetAll(), "rolId", "nombreRol", usuario.rolId);
            return View(usuario);
        }

        /// <summary>
        /// POST : edita un usuario. 
        /// </summary>
        /// <param name="id">usuario Id</param>
        /// Ok => Modifica registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Edit con mensaje de error.
        [AutorizarUsuario(permisoId: 2)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            var usuario = unitOfWork.Usuario.GetUsuarioActivoWhere(c => c.usuarioId == id);
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



        /// <summary>
        /// POST: modifica usuario => activo = 0 / rol  = no activo. 
        /// El usuario no se elimina en la BBDD
        /// para poder seguir consultando pólizas o gestiones realizadas por el usuario                
        /// </summary>
        /// <param name="usuarioId">usuarioId</param> 
        /// <return>
        /// Ok => Modifica registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Details con mensaje de error.
        /// </return>
        [AutorizarUsuario(permisoId: 3)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int usuarioId)
        {
            var usuario = unitOfWork.Usuario.GetUsuarioActivoWhere(c => c.usuarioId == usuarioId);
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            // Verifica que en la BBDD exista más de un usuario administrador activos ,
            // si solo hay un usuario administrador , no se permite eliminarlo.          
            if (usuario.rolId == 1)
            {
                var numeroAdmones = context.Usuario.Where(c => c.rolId == 1 && c.activo == 1).Count();
                if (numeroAdmones == 1)
                {
                    ViewBag.mensaje = ItemMensaje.ErrorEditarDesactivarUnicoAdministrador(Usuario.GetNombreModelo());
                    return View("Details", usuario);
                }
            }

            try
            {
                // Guarda la fecha de hoy como fecha de baja.
                DateTime hoy = DateTime.Now;
                usuario.fechaBaja = hoy;

                // Asigna rol No Operativo.
                usuario.rolId = 2;
                usuario.activo = 0;

                // Actualiza el registro en la BBDD.
                //context.Entry(usuario).State = EntityState.Modified;
                //context.SaveChanges();
                unitOfWork.Usuario.Update(usuario);
                unitOfWork.SaveChanges();

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
