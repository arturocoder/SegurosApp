using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.BLL;
using ProyectoSegurosFpDaw.Controllers.Helpers;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class GestionPolizasController : Controller
    {
        private ProyectoSegurosDbEntities context;
        private UnitOfWork unitOfWork;
        private GestionPolizaBLL gestionPolizaBLL;
        private GestionPolizasControllerHelper helper;
        public GestionPolizasController()
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
            gestionPolizaBLL = new GestionPolizaBLL(unitOfWork);
            helper = new GestionPolizasControllerHelper();
        }

        #region Actions

        /// <summary>
        /// GET : muestra formulario para buscar Pólizas y listado con los resultados.
        /// </summary>
        /// <returns>Vista con formulario y resultados.</returns>    
        [AutorizarUsuario(permisoId: 16)]
        [HttpGet]
        public ActionResult Index()
        {

            // Estadosession que se envía a la vista a través de ViewBag
            // para colapsar/mostrar la sección que corresponda.
            // 1=>Buscar Póliza
            // 2=>Resultados          
            var estadoSession = "1";

            //Comprueba que haya mensajes enviado desde otra action 
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }

            // Recupera lista de pólizas coincidentes 
            // si viene de la acción BuscarPólizas.
            if (TempData.ContainsKey("polizasCoincidentes"))
            {
                estadoSession = "2";
                var polizasCoincidentes = TempData["polizasCoincidentes"];
                ViewBag.polizasCoincidentes = polizasCoincidentes;
            }

            // Select list con Estados de póliza
            // 0 => no vigente
            // 1 => vigente
            // 2 => todos
            ViewBag.estadoPoliza = helper.GetSelectListEstadoPolizaConOpcionTodos();
            ViewBag.estadoSession = estadoSession;
            return View();
        }

        /// <summary>
        /// GET : busca la última gestión de cada póliza que coincida con los parámetros introducidos.
        /// </summary> 
        [HttpGet]
        [AutorizarUsuario(permisoId: 20)]
        public ActionResult BuscarPolizas(int? polizaId, string matricula, string dniCliente, string telefonoCliente, string fechaInicio, string fechaFinal, string estadoPoliza)
        {
            if (gestionPolizaBLL.IsValidSearching(fechaInicio, fechaFinal, estadoPoliza) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosBuscar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }

            try
            {
                PolizaSearchingFields searchingFields = gestionPolizaBLL.GetSearchingFields(polizaId, matricula, dniCliente, telefonoCliente, fechaInicio, fechaFinal, estadoPoliza);

                List<GestionPoliza> results = gestionPolizaBLL.SearchPolizas(searchingFields);

                TempData["polizasCoincidentes"] = results;
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionBuscar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }


        }

        [AutorizarUsuario(permisoId: 15)]
        [HttpGet]
        public ActionResult Details(int id)
        {
            GestionPoliza gestionPoliza = unitOfWork.GestionPoliza.GetGestionPolizaWithClienteCondicionadoTipoGestion(id);
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            return View(gestionPoliza);
        }


        [AutorizarUsuario(permisoId: 17)]
        [HttpGet]
        public ActionResult Historico(int id)
        {
            List<GestionPoliza> historicoPoliza = gestionPolizaBLL.GetHistoricoPoliza(id);
            if (historicoPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosBuscar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            ViewBag.historicoLista = historicoPoliza;
            ViewBag.polizaId = id;
            ViewBag.gestionPolizaIdLast = gestionPolizaBLL.GetLastGestionPoliza(id).gestionPolizaId;
            return View();
        }

        /// <summary>
        /// GET: formulario para crear una nueva póliza.
        /// </summary>
        /// <param name="clienteDni">NIF/NIE cliente</param>
        /// <returns>
        /// si ya existe el cliente => vista con formulario para crear póliza con datos del cliente.
        /// si no existe el cliente => Vista con formulario para crear cliente. 
        /// </returns>
        [AutorizarUsuario(permisoId: 12)]
        [HttpGet]
        public ActionResult Create(string clienteDni)
        {
            if (clienteDni.IsNullOrWhiteSpace())
            {
                TempData["mensaje"] = ItemMensaje.ErrorNifNoValidoCrearPoliza(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            clienteDni = clienteDni.Trim().ToUpperInvariant();
            Cliente cliente = unitOfWork.Cliente.Where(c => c.dniCliente == clienteDni).FirstOrDefault();
            if (cliente == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorNifNoExisteCrearPoliza(Poliza.GetNombreModelo(), clienteDni);
                return RedirectToAction("Create", "Clientes");
            }
            if (cliente.activo == 0)
            {
                TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Cliente.GetNombreModelo(), cliente.clienteId);
                return RedirectToAction("Index");
            }
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            ViewBag.cliente = cliente;
            ViewBag.condicionadoPolizaId = new SelectList(unitOfWork.CondicionadoPoliza.Where(c => c.activo == 1), "condicionadoPolizaId", "tipoCondicionado");
            return View();
        }

        /// <summary>
        /// POST : crea una nueva póliza, y la primera gestionPoliza asociada a esa póliza.
        /// </summary>    
        [AutorizarUsuario(permisoId: 12)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "matricula,marcaVehiculo,modeloVehiculo,fechaInicio,fechaFin,precio,observaciones,condicionadoPolizaId")] GestionPoliza gestionPoliza, string clienteId)
        {
            if (ModelState.IsValid == false || gestionPolizaBLL.FieldsFormatCreate(gestionPoliza, clienteId) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            Cliente cliente = unitOfWork.Cliente.Get(int.Parse(clienteId));
            if (gestionPolizaBLL.ValidarFormatoMatricula(gestionPoliza.matricula) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorValidarFormatoMatricula(Poliza.GetNombreModelo());
                return RedirectToAction("Create", new { clienteDni = cliente.dniCliente });
            }
            if (gestionPolizaBLL.ExistMatriculaInPolizasActivas(gestionPoliza.matricula) == true)
            {
                TempData["mensaje"] = ItemMensaje.ErrorValidarMatriculaDuplicada(Poliza.GetNombreModelo(), gestionPoliza.matricula);
                return RedirectToAction("Index");
            }
            // Validaciones de rangos de fecha.
            DateTime today = DateTime.Today;
            if (gestionPoliza.fechaInicio < today)
            {
                TempData["mensaje"] = ItemMensaje.ErrorFechaInicioMenorHoy(Poliza.GetNombreModelo());
                return RedirectToAction("Create", new { clienteDni = cliente.dniCliente });
            }
            if (gestionPoliza.fechaFin > gestionPoliza.fechaInicio.AddYears(1))
            {
                TempData["mensaje"] = ItemMensaje.ErrorFechasMaxRangoInicioFin(Poliza.GetNombreModelo(), 365);
                return RedirectToAction("Create", new { clienteDni = cliente.dniCliente });
            }
            if (gestionPoliza.fechaFin > today.AddYears(1).AddMonths(6))
            {
                TempData["mensaje"] = ItemMensaje.ErrorFechasMaxRangoHoyFin(Poliza.GetNombreModelo(), 547);
                return RedirectToAction("Create", new { clienteDni = cliente.dniCliente });
            }
            try
            {
                var usuario = GetUsuarioLogado();
                Poliza polizaCreada = gestionPolizaBLL.CreatePolizaAndFirstGestionPoliza(gestionPoliza, usuario, cliente);
                TempData["mensaje"] = ItemMensaje.SuccessCrear(Poliza.GetNombreModelo(), polizaCreada.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                gestionPolizaBLL.UnCreatePoliza(cliente);
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Create", new { clienteDni = cliente.dniCliente });
            }

        }

        /// <summary>
        /// GET: formulario para modificar una póliza => crea una nueva gestión póliza. 
        /// </summary>
        /// <param name="id">polizaId</param>
        /// <returns>Vista con formulario para modificar una póliza (gestión póliza)</returns>
        [AutorizarUsuario(permisoId: 13)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            GestionPoliza gestionPoliza = gestionPolizaBLL.GetLastGestionPoliza(id);
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            IEnumerable<CondicionadoPoliza> condicionadosActivos = unitOfWork.CondicionadoPoliza.Where(c => c.activo == 1);
            ViewBag.condicionadoPolizaId = new SelectList(condicionadosActivos, "condicionadoPolizaId", "tipoCondicionado", gestionPoliza.condicionadoPolizaId);
            return View(gestionPoliza);
        }

        /// <summary>
        /// POST: Crea una nueva gestión póliza asociada a la póliza con las modificaciones realizadas
        /// </summary>
        /// <param name="id">polizaId</param>             
        [AutorizarUsuario(permisoId: 13)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            GestionPoliza gestionPoliza = gestionPolizaBLL.GetLastGestionPoliza(id);
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            var matriculaEstadoPrevio = gestionPoliza.matricula;
            if (TryUpdateModel(gestionPoliza, "", new string[] { "condicionadoPolizaId", "precio", "observaciones", "matricula", "marcaVehiculo", "modeloVehiculo" }) == false || gestionPolizaBLL.FieldsFormatEdit(gestionPoliza) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
            }

            if (gestionPolizaBLL.ValidarFormatoMatricula(gestionPoliza.matricula) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorValidarFormatoMatricula(Poliza.GetNombreModelo());
                return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
            }
            // Si se ha modificado la matrícula, verifica que no esté en una póliza Activa.                     
            if (matriculaEstadoPrevio != gestionPoliza.matricula && gestionPolizaBLL.ExistMatriculaInPolizasActivas(gestionPoliza.matricula) == true)
            {
                TempData["mensaje"] = ItemMensaje.ErrorValidarMatriculaDuplicada(Poliza.GetNombreModelo(), gestionPoliza.matricula);
                return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
            }
            try
            {
                Usuario usuarioLogado = GetUsuarioLogado();
                gestionPolizaBLL.UpdateGestionPoliza(gestionPoliza, usuarioLogado);
                TempData["mensaje"] = ItemMensaje.SuccessEditar(Poliza.GetNombreModelo(), gestionPoliza.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                ViewBag.mensaje = ItemMensaje.ErrorExcepcionEditar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                IEnumerable<CondicionadoPoliza> condicionadosActivos = unitOfWork.CondicionadoPoliza.Where(c => c.activo == 1);
                ViewBag.condicionadoPolizaId = new SelectList(condicionadosActivos, "condicionadoPolizaId", "tipoCondicionado", gestionPoliza.condicionadoPolizaId);
                return View(gestionPoliza);
            }
        }


        /// <summary>
        /// POST : cancela una póliza, modificando su estado activo a 0, 
        /// y crea una nueva gestión póliza modificando su fecha de fin.
        /// </summary>      
        [AutorizarUsuario(permisoId: 14)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int gestionPolizaId, string motivoClx)
        {
            if (motivoClx.IsNullOrWhiteSpace())
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCancelar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            GestionPoliza gestionPoliza = unitOfWork.GestionPoliza.Get(gestionPolizaId);
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCancelar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            Poliza poliza = unitOfWork.Poliza.Get(gestionPoliza.polizaId);
            if (poliza.activo == 0)
            {
                TempData["mensaje"] = ItemMensaje.ErrorYaCanceladoOrDesactivado(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            try
            {
                gestionPolizaBLL.DeleteGestionPoliza(gestionPoliza, GetUsuarioLogado(), motivoClx, poliza);
                TempData["mensaje"] = ItemMensaje.SuccessCancelar(Poliza.GetNombreModelo(), poliza.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                gestionPolizaBLL.UnDeleteGestionPoliza(poliza.polizaId);
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCancelar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }
        #endregion
        #region Métodos
        /// <summary>
        /// Obtiene el Usuario de la Session actual (usuario logado).
        /// </summary>
        /// <returns>
        /// usuario
        /// </returns>
        private Usuario GetUsuarioLogado()
        {
            Usuario oUsuario = (Usuario)Session["user"];
            return oUsuario;
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
