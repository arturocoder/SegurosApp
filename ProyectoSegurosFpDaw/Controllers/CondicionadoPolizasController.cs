using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.BLL;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class CondicionadoPolizasController : Controller
    {      
        private ProyectoSegurosDbEntities context;
        private UnitOfWork unitOfWork;
        private CondicionadoPolizaBLL condicionadoPolizaBLL;

        public CondicionadoPolizasController()
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
            condicionadoPolizaBLL = new CondicionadoPolizaBLL(unitOfWork);
        }
        #region Actions

        [AutorizarUsuario(permisoId: 23)]
        [HttpGet]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            ViewBag.listaCondicionados = unitOfWork.CondicionadoPoliza.GetAll();

            return View();
        }

        [AutorizarUsuario(permisoId: 22)]
        [HttpGet]
        public ActionResult IndexNoEditable()
        {
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            ViewBag.listaCondicionados = unitOfWork.CondicionadoPoliza.GetAll();

            return View();
        }

        [AutorizarUsuario(permisoId: 23)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tipoCondicionado,garantias")] CondicionadoPoliza condicionadoPoliza)
        {
            if (ModelState.IsValid == false || condicionadoPolizaBLL.FieldsFormat(condicionadoPoliza) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }           
            if (condicionadoPolizaBLL.AnyCondicionadoWithTipoCondicionado(condicionadoPoliza.tipoCondicionado))
            {
                TempData["mensaje"] = ItemMensaje.ErrorRegistroDuplicadoCrear(CondicionadoPoliza.GetNombreModelo(), "tipo de condicionado", null);
                return RedirectToAction("Index");
            }
            try
            {
                condicionadoPolizaBLL.CreateNewCondicionadoPoliza(condicionadoPoliza);
                TempData["mensaje"] = ItemMensaje.SuccessCrear(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(CondicionadoPoliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }

        [AutorizarUsuario(permisoId: 23)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int IDcondicionado)
        {
            CondicionadoPoliza condicionadoPoliza = unitOfWork.CondicionadoPoliza.Get(IDcondicionado);           
            string tipoCondicionadoAntiguo = condicionadoPoliza.tipoCondicionado;        

            if (condicionadoPoliza == null || TryUpdateModel(condicionadoPoliza, "", new string[] { "tipoCondicionado", "garantias" }) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }            
            if (condicionadoPolizaBLL.FieldsFormat(condicionadoPoliza) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(CondicionadoPoliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (condicionadoPoliza.tipoCondicionado != tipoCondicionadoAntiguo
                && condicionadoPolizaBLL.AnyCondicionadoWithTipoCondicionado(condicionadoPoliza.tipoCondicionado))
            {
                TempData["mensaje"] = ItemMensaje.ErrorRegistroDuplicadoEditar(CondicionadoPoliza.GetNombreModelo(), "tipo de condicionado", null);
                return RedirectToAction("Index");
            }
            try
            {
                unitOfWork.CondicionadoPoliza.Update(condicionadoPoliza);
                unitOfWork.SaveChanges();
                TempData["mensaje"] = ItemMensaje.SuccessEditar(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);
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
                     where gestiones.condicionadoPolizaId == condicionadoPoliza.condicionadoPolizaId && polizas.activo == 1
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
                            .Where(c => c.polizaId == item.Poliza)
                            .OrderByDescending(c => c.gestionPolizaId)
                            .FirstOrDefault();

                        // Si coincide con el condicionadoId obtenido del post, añade al listado de polizasId.
                        if (ultimaGestion.condicionadoPolizaId == condicionadoPoliza.condicionadoPolizaId)
                        {
                            polizasIdList.Add(ultimaGestion.polizaId);

                        }
                    }
                    // Si la lista de pólizas coincidentes tiene algún registro ,
                    // devuelve msj de error con el listado de pólizas vigentes con ese tipo de condicionado.
                    if (polizasIdList.Any())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorPolizaVigorDesactivarCondicionado(CondicionadoPoliza.GetNombreModelo(), polizasIdList);
                        return RedirectToAction("Index");
                    }
                }
                try
                {

                    DateTime hoy = DateTime.Now;
                    condicionadoPoliza.fechaDesactivado = hoy;
                    condicionadoPoliza.activo = 0;
                    unitOfWork.CondicionadoPoliza.Update(condicionadoPoliza);
                    unitOfWork.SaveChanges();

                    TempData["mensaje"] = ItemMensaje.SuccessDesactivar(CondicionadoPoliza.GetNombreModelo(), condicionadoPoliza.tipoCondicionado);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorExcepcionDesactivar(CondicionadoPoliza.GetNombreModelo(), ex.GetType().ToString());
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
                    unitOfWork.CondicionadoPoliza.Update(condicionadoPoliza);
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
