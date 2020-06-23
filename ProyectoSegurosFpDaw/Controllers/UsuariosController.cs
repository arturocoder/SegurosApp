﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class UsuariosController : Controller
    {
        // Instancia de la BBDD
        private ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities();

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
            ViewBag.rolId = GetSelectListRolConOpcionTodos();
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
                        var UsuariosCoincidentes = db.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1)
                           .ToList();
                        TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var UsuariosCoincidentes = db.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.rolId == rolID)
                           .ToList();
                        TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
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
                            var UsuariosCoincidentes = db.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.nombreUsuario == nombreUsuario && c.apellido1Usuario == apellido1Usuario)
                               .ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            var UsuariosCoincidentes = db.Usuario
                            .Include(c => c.Rol)
                            .Where(c => c.activo == 1 && c.nombreUsuario == nombreUsuario && c.apellido1Usuario == apellido1Usuario && c.rolId == rolID)
                            .ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                    }
                    // Nombre + Rol.
                    else if (nombreUsuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            var UsuariosCoincidentes = db.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.nombreUsuario == nombreUsuario)
                               .ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var UsuariosCoincidentes = db.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.nombreUsuario == nombreUsuario && c.rolId == rolID).ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                    }
                    // Apellido + Rol.
                    else if (apellido1Usuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            var UsuariosCoincidentes = db.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.apellido1Usuario == apellido1Usuario)
                               .ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            var UsuariosCoincidentes = db.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.apellido1Usuario == apellido1Usuario && c.rolId == rolID).ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                    }
                    // NIF/NIE.
                    else if (dniUsuario.Length > 0)
                    {
                        // Todos los roles.
                        if (rolId == "0")
                        {
                            var UsuariosCoincidentes = db.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.dniUsuario == dniUsuario)
                               .ToList();

                            // Si no hay coincidencia en cliente activo, busca en clientes no activos,
                            // y envía mensaje si hay coincidencia.
                            if (UsuariosCoincidentes.Count == 0)
                            {
                                var usuarioNoActivo = db.Usuario
                                 .Where(c => c.activo == 0 && c.dniUsuario == dniUsuario).FirstOrDefault();
                                if (usuarioNoActivo != null)
                                {
                                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Usuario.GetNombreModelo(), usuarioNoActivo.usuarioId);
                                }
                            }
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var UsuariosCoincidentes = db.Usuario
                            .Include(c => c.Rol)
                            .Where(c => c.activo == 1 && c.dniUsuario == dniUsuario && c.rolId == rolID).ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                    }
                    // Email.
                    else if (emailUsuario.Length > 0)
                    {
                        //Todos los roles.
                        if (rolId == "0")
                        {
                            var UsuariosCoincidentes = db.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.emailUsuario == emailUsuario)
                               .ToList();
                            if (UsuariosCoincidentes.Count == 0)
                            {
                                var usuarioNoActivo = db.Usuario
                                 .Where(c => c.activo == 0 && c.emailUsuario == emailUsuario).FirstOrDefault();
                                if (usuarioNoActivo != null)
                                {
                                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Usuario.GetNombreModelo(), usuarioNoActivo.usuarioId);
                                }
                            }
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var UsuariosCoincidentes = db.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.emailUsuario == emailUsuario && c.rolId == rolID).ToList();
                            TempData["usuariosCoincidentes"] = UsuariosCoincidentes;
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

        /// <summary>
        /// GET: Muestra la información de un usuario.
        /// </summary>
        /// <param name="id">usuario Id</param>
        /// <returns>Vista de la información del usuario</returns>
        [HttpGet]
        [AutorizarUsuario(permisoId: 4)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            Usuario usuario = db.Usuario
                .Include(c => c.Rol)
                .Where(w => w.usuarioId == id && w.activo == 1)
                .SingleOrDefault();
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        /// <summary>
        /// GET: formulario para crear un nuevo usuario.
        /// </summary>
        /// <returns>Vista con formulario para crear usuario</returns>
        [HttpGet]
        [AutorizarUsuario(permisoId: 1)]
        public ActionResult Create()
        {
            ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol");
            return View();
        }

        /// <summary>
        /// POST:  crea un nuevo usuario.
        /// </summary>
        /// <param name="usuario">usuario con :nombre, apellido1 apellido2,NIF/NIE,email
        /// ,password,rolId</param>
        /// <returns>
        /// Ok => Guarda registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Create con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 1)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombreUsuario,apellido1Usuario,apellido2Usuario,dniUsuario,emailUsuario,password,rolId")] Usuario usuario)
        {
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validaciones y formato de parámetros.
                    if (usuario.nombreUsuario.IsNullOrWhiteSpace() || usuario.apellido1Usuario.IsNullOrWhiteSpace()
                        || usuario.apellido2Usuario.IsNullOrWhiteSpace()
                        || usuario.dniUsuario.IsNullOrWhiteSpace() || usuario.emailUsuario.IsNullOrWhiteSpace()
                        || usuario.password.IsNullOrWhiteSpace())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Usuario.GetNombreModelo());
                        return RedirectToAction("Index");
                    }

                    usuario.nombreUsuario = usuario.nombreUsuario.Trim().ToUpperInvariant();
                    usuario.apellido1Usuario = usuario.apellido1Usuario.Trim().ToUpperInvariant();
                    usuario.apellido2Usuario = usuario.apellido2Usuario.Trim().ToUpperInvariant();
                    usuario.dniUsuario = usuario.dniUsuario.Trim().ToUpperInvariant();
                    usuario.emailUsuario = usuario.emailUsuario.Trim().ToUpperInvariant();

                    if (VerificarDniDuplicadoBack(usuario.dniUsuario) == 1)
                    {
                        ViewBag.mensaje = ItemMensaje.ErrorRegistroDuplicadoCrear(Usuario.GetNombreModelo(), "NIF/NIE", null);
                        ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol");
                        return View(usuario);
                    }
                    if (VerificarEmailDuplicadoBack(usuario.emailUsuario) == 1)
                    {
                        ViewBag.mensaje = ItemMensaje.ErrorRegistroDuplicadoCrear(Usuario.GetNombreModelo(), "Email", null);
                        ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol");
                        return View(usuario);
                    }

                    // Guarda la fecha de hoy como fecha de alta.
                    DateTime hoy = DateTime.Now;
                    usuario.fechaAlta = hoy;
                    usuario.activo = 1;
                    // Encripta la password.
                    var psw = usuario.password.Trim();
                    var pswEncriptada = Encriptacion.GetSHA256(psw);
                    usuario.password = pswEncriptada;

                    // Guarda el registro en la BBDD.
                    db.Usuario.Add(usuario);
                    db.SaveChanges();
                    TempData["mensaje"] = ItemMensaje.SuccessCrear(Usuario.GetNombreModelo(), usuario.dniUsuario);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.mensaje = ItemMensaje.ErrorExcepcionCrear(Usuario.GetNombreModelo(), ex.GetType().ToString());
                    ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol");
                    return View(usuario);
                }
            }
            TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Usuario.GetNombreModelo());
            return RedirectToAction("Index");
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
            Usuario usuario = db.Usuario
                .Include(c => c.Rol)
                .Where(w => w.usuarioId == id && w.activo == 1)
                .SingleOrDefault();
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }
            ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol", usuario.rolId);
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
            Usuario usuario = db.Usuario.Where(c => c.activo == 1 && c.usuarioId == id).FirstOrDefault();
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            try
            {
                // Intenta actualizar el usuario con los datos enviados desde el formulario.
                if (TryUpdateModel(usuario, "", new string[] { "nombreUsuario", "apellido1Usuario", "apellido2Usuario", "emailUsuario", "password", "rolId" }))
                {
                    // Validaciones y formato de párametros.
                    if (usuario.nombreUsuario.IsNullOrWhiteSpace() || usuario.apellido1Usuario.IsNullOrWhiteSpace()
                        || usuario.apellido2Usuario.IsNullOrWhiteSpace() || usuario.dniUsuario.IsNullOrWhiteSpace()
                        || usuario.emailUsuario.IsNullOrWhiteSpace() || usuario.password.IsNullOrWhiteSpace())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                        return RedirectToAction("Index");
                    }

                    // Si solo hay un usuario con rol Administrador en la BBDD.
                    if (ComprobarUnicoAdministrador() == true)
                    {
                        // Comprueba si el usuario a editar tiene un rol administrador .
                        var usuarioEstadoPrevio = db.Usuario.AsNoTracking()
                            .Where(c => c.usuarioId == usuario.usuarioId && c.rolId == 1).FirstOrDefault();

                        // Comprueba si está cambiando el rol a otro diferente de administrador.
                        if (usuarioEstadoPrevio != null && usuarioEstadoPrevio.rolId != usuario.rolId)
                        {
                            ViewBag.mensaje = ItemMensaje.ErrorEditarDesactivarUnicoAdministrador(Usuario.GetNombreModelo());
                            ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol", usuario.rolId);
                            return View(usuario);
                        }

                    }
                    // Si se va a modificar rol a No operativo.                   
                    if (usuario.rolId == 2)
                    {
                        // Guarda la fecha de hoy como fecha de baja.
                        DateTime hoy = DateTime.Now;
                        usuario.fechaBaja = hoy;
                    }
                    else
                    {
                        usuario.fechaBaja = null;
                    }

                    usuario.nombreUsuario = usuario.nombreUsuario.Trim().ToUpperInvariant();
                    usuario.apellido1Usuario = usuario.apellido1Usuario.Trim().ToUpperInvariant();
                    usuario.apellido2Usuario = usuario.apellido2Usuario.Trim().ToUpperInvariant();
                    usuario.emailUsuario = usuario.emailUsuario.Trim().ToUpperInvariant();
                    // Encripta la password.
                    var psw = usuario.password.Trim();
                    var pswEncriptada = Encriptacion.GetSHA256(psw);
                    usuario.password = pswEncriptada;

                    // Guarda las modificaciones en la BBDD.
                    db.SaveChanges();
                    TempData["mensaje"] = ItemMensaje.SuccessEditar(Usuario.GetNombreModelo(), usuario.apellido1Usuario);
                    return RedirectToAction("Index");
                }
                ViewBag.mensaje = ItemMensaje.ErrorDatosNoValidosEditar(Usuario.GetNombreModelo());
                ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol", usuario.rolId);
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ItemMensaje.ErrorExcepcionEditar(Usuario.GetNombreModelo(), ex.GetType().ToString());
                ViewBag.rolId = new SelectList(db.Rol, "rolId", "nombreRol", usuario.rolId);
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
            Usuario usuario = db.Usuario.Where(c => c.activo == 1 && c.usuarioId == usuarioId).FirstOrDefault();
            if (usuario == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivar(Usuario.GetNombreModelo());
                return RedirectToAction("Index");
            }

            // Verifica que en la BBDD exista más de un usuario administrador activos ,
            // si solo hay un usuario administrador , no se permite eliminarlo.          
            if (usuario.rolId == 1)
            {
                var numeroAdmones = db.Usuario.Where(c => c.rolId == 1 && c.activo == 1).Count();
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
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
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
        #region Métodos

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
            var respuestaJson = 1;
            var nif = dni.Trim().ToUpperInvariant();
            var usuarioCoincidente = db.Usuario
                   .Where(c => c.dniUsuario == dni).FirstOrDefault();

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
        /// Comprueba si el NIF / NIE introducido
        /// existe en Usuario.  
        /// </summary>
        /// <param name="dni">NIF/NIE usuario</param>
        /// <returns> 
        /// 1 => hay coincidencia.
        /// 0 => no hay coincidencia.
        /// </returns>
        private int VerificarDniDuplicadoBack(string dni)
        {
            var respuestaJson = 1;
            var nif = dni.Trim().ToUpperInvariant();
            var usuarioCoincidente = db.Usuario
                   .Where(c => c.dniUsuario == dni).FirstOrDefault();
            if (usuarioCoincidente != null)
            {
                respuestaJson = 1;
            }
            else
            {
                respuestaJson = 0;
            }
            return respuestaJson;
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
            var respuestaJson = 1;
            var mail = email.Trim().ToUpperInvariant();
            var usuarioCoincidente = db.Usuario
                   .Where(c => c.emailUsuario == mail).FirstOrDefault();
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
        /// Comprueba si el email introducido
        /// existe en Usuario.  
        /// </summary>
        /// <param name="dni">email usuario</param>
        /// <returns> 
        /// 1 => hay coincidencia.
        /// 0 => no hay coincidencia.
        /// </returns>
        private int VerificarEmailDuplicadoBack(string email)
        {
            var respuesta = 1;
            var mail = email.Trim().ToUpperInvariant();
            var usuarioCoincidente = db.Usuario
                   .Where(c => c.emailUsuario == mail).FirstOrDefault();
            if (usuarioCoincidente != null)
            {
                respuesta = 1;
            }
            else
            {
                respuesta = 0;
            }
            return respuesta;
        }

        /// <summary>
        /// Añade al selectList de Roles , la opción con valor 0 y texto Todos
        /// para poder seleccionar Todos en el DropDown list de la Vista
        /// </summary>
        /// <returns> retorna un List<SelectListItem> </returns>
        private List<SelectListItem> GetSelectListRolConOpcionTodos()
        {
            var roles = new List<SelectListItem>();
            roles.Add(new SelectListItem { Value = "0", Text = "TODOS" });
            var rolesT = db.Rol.Where(p => p.activo == 1).OrderBy(p => p.nombreRol);
            foreach (var item in rolesT)
            {
                roles.Add(new SelectListItem { Value = item.rolId.ToString(CultureInfo.GetCultureInfo("es-ES")), Text = item.nombreRol });
            }
            return roles;
        }

        /// <summary>
        /// Comprueba si solo existe un unico usuario con rol Administrador en la BBDD.
        /// </summary>
        /// <returns>
        /// true=> solo hay un usuario con rol Administrador
        /// false=> hay más de un usuario con rol Administrador
        /// </returns>
        private bool ComprobarUnicoAdministrador()
        {
            var numeroAdmones = db.Usuario.Where(c => c.rolId == 1 && c.activo == 1).Count();
            if (numeroAdmones == 1) { return true; } else { return false; }

        }

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
