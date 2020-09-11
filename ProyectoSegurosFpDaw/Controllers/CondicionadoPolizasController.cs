using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class CondicionadoPolizasController : Controller
    {
        // Instancia de la BBDD.
        private ProyectoSegurosDbEntities context = new ProyectoSegurosDbEntities();       

        #region Actions

        /// <summary>
        /// GET : Muestra lista de Condicionado Pólizas
        /// sin opción de crear / editar / activar / desactivar.
        /// </summary>
        /// <returns>Vista con lista de CondicionadosPólizas.</returns>
        [AutorizarUsuario(permisoId: 23)]
        [HttpGet]
        public ActionResult Index()
        {            
            // Comprueba que haya mensajes enviado desde otra action y lo envía a la vista.
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            var unitOfWork = new UnitOfWork(context);           
            ViewBag.listaCondicionados = unitOfWork.CondicionadoPoliza.GetAll();

            return View();
        }

        /// <summary>
        /// GET : Muestra lista de Condicionado Pólizas
        /// con opción de crear / editar /  activar / desactivar.
        /// </summary>
        /// <returns>Vista con lista de CondicionadosPólizas.</returns>
        [AutorizarUsuario(permisoId: 22)]
        [HttpGet]
        public ActionResult IndexNoEditable()
        {
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            var unitOfWork = new UnitOfWork(context);
            ViewBag.listaCondicionados = unitOfWork.CondicionadoPoliza.GetAll();
         
            return View();
        }       

        /// <summary>
        /// POST : Crea un nuevo condicionado de Póliza.
        /// </summary>
        /// <param name="condicionadoPoliza">tipoCondicionado/garantías</param>
        /// <returns>
        /// Ok => Guarda registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 23)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tipoCondicionado,garantias")] CondicionadoPoliza condicionadoPoliza)
        {
            if (condicionadoPoliza == null)
            {               
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                try
                {   
                    // Validaciones y formato de los campos.
                    if(condicionadoPoliza.tipoCondicionado.IsNullOrWhiteSpace()|| condicionadoPoliza.garantias.IsNullOrWhiteSpace())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(CondicionadoPoliza.GetNombreModelo());
                        return RedirectToAction("Index");
                    }                   
                    condicionadoPoliza.tipoCondicionado = condicionadoPoliza.tipoCondicionado.Trim().ToUpperInvariant();
                    condicionadoPoliza.garantias = condicionadoPoliza.garantias.Trim();
                                      
                    if (VerificarCondicionadoDuplicado(condicionadoPoliza.tipoCondicionado) == 1)
                    {                       
                        TempData["mensaje"] = ItemMensaje.ErrorRegistroDuplicadoCrear(CondicionadoPoliza.GetNombreModelo(), "tipo de condicionado",null);
                        return RedirectToAction("Index");
                    }
                    
                    // Activo = 1 => Condicionado vigente // 0 => no vigente.
                    condicionadoPoliza.activo = 1;                   
                    var unitOfWork = new UnitOfWork(context);
                    unitOfWork.CondicionadoPoliza.Add(condicionadoPoliza);
                    unitOfWork.SaveChanges();     
                    
                    TempData["mensaje"] = ItemMensaje.SuccessCrear(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);                 
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {                    
                    TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(CondicionadoPoliza.GetNombreModelo(),ex.GetType().ToString());
                    return RedirectToAction("Index");
                }
            }            
            TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(CondicionadoPoliza.GetNombreModelo());
            return RedirectToAction("Index");
        }

        /// <summary>
        /// POST : edita un condicionado de póliza.
        /// </summary>
        /// <param name="IDcondicionado">condicionadoId</param>
        /// <returns>
        /// Ok => Modifica registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 23)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int IDcondicionado)
        {
            //if (IDcondicionado == null)
            //{             
            //    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
            //    return RedirectToAction("Index");
            //}
            UnitOfWork unitOfWork = new UnitOfWork(context);
            var condicionadoPoliza = unitOfWork.CondicionadoPoliza.Get(IDcondicionado);

            if (condicionadoPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            
            // Obtiene el nombre del condicionado antes de editar, para poder realizar validaciones.
            var nombreCondicionadoAntiguo = condicionadoPoliza.tipoCondicionado.Trim().ToUpperInvariant();
            try
            {                
                // Intenta actualizar el condicionado con los datos enviados en el formulario post 
                if (TryUpdateModel(condicionadoPoliza, "", new string[] { "tipoCondicionado","garantias" }))
                {
                    // Validaciones y formato de los campos
                    if (condicionadoPoliza.tipoCondicionado.IsNullOrWhiteSpace() || condicionadoPoliza.garantias.IsNullOrWhiteSpace())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
                        return RedirectToAction("Index");
                    }

                    condicionadoPoliza.tipoCondicionado = condicionadoPoliza.tipoCondicionado.Trim().ToUpperInvariant();
                    condicionadoPoliza.garantias = condicionadoPoliza.garantias.Trim();                                 
                                       
                    // Si se ha modificado el tipoCondicionado (nombre).
                    if (condicionadoPoliza.tipoCondicionado != nombreCondicionadoAntiguo)
                    {                                              
                        if (VerificarCondicionadoDuplicado(condicionadoPoliza.tipoCondicionado) == 1)
                        {
                            TempData["mensaje"] = ItemMensaje.ErrorRegistroDuplicadoEditar(CondicionadoPoliza.GetNombreModelo(), "tipo de condicionado", null);
                            return RedirectToAction("Index");
                        }
                    }                                    

                    // Actualiza registro en la BBDD.
                    unitOfWork.SaveChanges();

                    TempData["mensaje"] = ItemMensaje.SuccessEditar(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);
                    return RedirectToAction("Index");
                }
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }           
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionEditar(CondicionadoPoliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST : Activa y desactiva Condicionado Póliza (vigente / no vigente) 
        /// <para>Activo = 1 (vigente) / 
        /// Activo = 0 (no vigente).</para>
        /// </summary>
        /// <param name="condicionadoId"> condicionadoId</param>
        /// <returns>
        ///  Ok => Modifica registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 23)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int condicionadoId)
        {
            UnitOfWork unitOfWork = new UnitOfWork(context);
            CondicionadoPoliza condicionadoPoliza = unitOfWork.CondicionadoPoliza.Get(condicionadoId);

            if (condicionadoPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivarActivar(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            //Si condicionado está activo, lo desactiva.
            if (condicionadoPoliza.activo == 1)
            {
                // Validación :  no se puede desactivar condicionado si alguna póliza en vigor tiene ese tipo de Condicionado.

                // Obtiene los polizasId de las pólizas que coinciden que estén activas y que tengan alguna gestiónPóliza con el condicionadoId.
                var polizasCoincidentes =
                     from gestiones in context.GestionPoliza
                     join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                     where gestiones.condicionadoPolizaId == condicionadoPoliza.condicionadoPolizaId && polizas.activo ==1
                     select new { Poliza = polizas.polizaId };

                // Si hay alguna coincidencia,comprueba que sea la última gestiónPóliza de la póliza.
                if (polizasCoincidentes.Any())
                {                    
                    var polizasIdList = new List<int>();
                    // Recorre las pólizas coincidentes descartando las repetidas.
                    foreach (var item in polizasCoincidentes.Distinct())
                    {
                        // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                        var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza)
                            .Where(c=> c.polizaId == item.Poliza )
                            .OrderByDescending(c => c.gestionPolizaId)
                            .FirstOrDefault();                            
                        
                        // Si coincide con el condicionadoId obtenido del post, añade al listado de polizasId.
                        if (ultimaGestion.condicionadoPolizaId==condicionadoPoliza.condicionadoPolizaId)
                        {
                            polizasIdList.Add(ultimaGestion.polizaId);

                        }
                    }
                    // Si la lista de pólizas coincidentes tiene algún registro ,
                    // devuelve msj de error con el listado de pólizas vigentes con ese tipo de condicionado.
                    if (polizasIdList.Any()) 
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorPolizaVigorDesactivarCondicionado(CondicionadoPoliza.GetNombreModelo(),polizasIdList);
                        return RedirectToAction("Index");
                    }
                }
                try
                {
                    // Guarda la fecha de hoy como fecha de baja.
                    DateTime hoy = DateTime.Now;
                    condicionadoPoliza.fechaDesactivado = hoy;
                    condicionadoPoliza.activo = 0;
                    // Actualiza registro en la BBDD
                    //context.Entry(condicionadoPoliza).State = EntityState.Modified;
                    //context.SaveChanges();
                    unitOfWork.SaveChanges();

                    TempData["mensaje"] = ItemMensaje.SuccessDesactivar(CondicionadoPoliza.GetNombreModelo(),condicionadoPoliza.tipoCondicionado);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorExcepcionDesactivar(CondicionadoPoliza.GetNombreModelo(),ex.GetType().ToString());
                    return RedirectToAction("Index");
                }
            }
            // Si el condicionado no está activo, lo activa.
            else if (condicionadoPoliza.activo == 0)
            {
                try
                {                    
                    condicionadoPoliza.fechaDesactivado = null;
                    condicionadoPoliza.activo = 1;
                    //context.Entry(condicionadoPoliza).State = EntityState.Modified;
                    //context.SaveChanges();
                    unitOfWork.SaveChanges();
                    TempData["mensaje"] = ItemMensaje.SuccessActivar(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorExcepcionActivar(CondicionadoPoliza.GetNombreModelo(), ex.GetType().ToString());
                    return RedirectToAction("Index");
                }
            }
            TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivarActivar(CondicionadoPoliza.GetNombreModelo());
            return RedirectToAction("Index");
        }
        #endregion

        #region Métodos

        /// <summary>
        /// Comprueba que el nombre del Condicionado (tipoCondicionado)
        /// no exista en la BBDD
        /// </summary>
        /// <param name="nombreCondicionado">tipoCondicionado</param>
        /// <returns>  
        /// 1 => tipoCondicionado ya existe en la BBDD.
        /// 0 => tipoCondicionado  no existe en BBDD.
        /// </returns>
        private int VerificarCondicionadoDuplicado(string nombreCondicionado)
        {
            var respuesta = 1;
            var condicionado = nombreCondicionado.Trim().ToUpperInvariant();
            var condicionadoCoincidente = context.CondicionadoPoliza
                   .Where(c => c.tipoCondicionado == condicionado).FirstOrDefault();


            if (condicionadoCoincidente != null)
            {
                respuesta = 1;
            }
            else
            {
                respuesta = 0;
            }
            return respuesta;
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
