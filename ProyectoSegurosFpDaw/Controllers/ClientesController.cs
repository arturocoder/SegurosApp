using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.BLL;
using ProyectoSegurosFpDaw.Filtros;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;

namespace ProyectoSegurosFpDaw.Controllers
{
    [RequireHttps]
    public class ClientesController : Controller
    {
        private ProyectoSegurosDbEntities context;

        private UnitOfWork unitOfWork;
        private ClienteBLL clienteBll;

        public ClientesController()
        {
            context = new ProyectoSegurosDbEntities();
            unitOfWork = new UnitOfWork(context);
            clienteBll = new ClienteBLL(unitOfWork);
        }
        #region Actions

        /// <summary>
        /// GET : muestra formulario para buscar clientes y listado con los resultados.
        /// </summary>
        /// <returns>Vista con formulario y resultados.</returns>
        [AutorizarUsuario(permisoId: 10)]
        [HttpGet]
        public ActionResult Index()
        {
            // Estadosession que se envía a la vista a través de ViewBag
            // para colapsar/mostrar la sección que corresponda.
            // 1=>Buscar Cliente
            // 2=>Resultados            
            var estadoSession = "1";

            // Comprueba que haya mensajes enviado desde otra action.
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }

            // Recupera lista de clientes coincidentes si viene de la acción BuscarClientes.
            if (TempData.ContainsKey("clientesCoincidentes"))
            {
                estadoSession = "2";
                var clientesCoincidentes = TempData["clientesCoincidentes"];
                ViewBag.clientesCoincidentes = clientesCoincidentes;
            }
            ViewBag.estadoSession = estadoSession;
            return View();
        }

        /// <summary>
        /// GET : busca en la BBDD Clientes que coincidan con los parámetros introducidos.
        /// </summary>
        /// <param name="clienteId">cliente Id</param>
        /// <param name="dniCliente">NIF/NIE Cliente</param>
        /// <param name="emailCliente">email Cliente</param>
        /// <param name="telefonoCliente">teléfono Cliente</param>
        /// <returns>
        /// Hay coincidencias de clientes activos => envía una lista Cliente de coincidencias y redirecciona al Index para mostrarlos.
        /// Hay coincidencias de clientes no activos => envía un mensaje de información y redirecciona al Index.
        /// Sin coincidencias => envía una lista Cliente vacía y redirecciona a Index .
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 19)]
        [HttpGet]
        public ActionResult BuscarClientes(string clienteId, string dniCliente, string emailCliente, string telefonoCliente)
        {
            ClienteParam searchingField = clienteBll.GetSearchingField(clienteId, dniCliente, emailCliente, telefonoCliente);

            List<Cliente> clienteMatches = new List<Cliente>();

            if (searchingField == ClienteParam.id)
            {
                clienteMatches = clienteBll.SearchClientes(ClienteParam.id, clienteId);
                // Si cliente ha sido eliminado
                if (clienteMatches.Any(c => c.activo == 0))
                {
                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Cliente.GetNombreModelo(), clienteMatches.FirstOrDefault().clienteId);
                }
            }
            if (searchingField == ClienteParam.dni)
            {
                clienteMatches = clienteBll.SearchClientes(ClienteParam.dni, dniCliente);
                if (clienteMatches.Any(c => c.activo == 0))
                {
                    TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Cliente.GetNombreModelo(), clienteMatches.FirstOrDefault().clienteId);
                }
            }
            if (searchingField == ClienteParam.email)
            {
                clienteMatches = clienteBll.SearchClientes(ClienteParam.email, emailCliente);
            }
            if (searchingField == ClienteParam.telefono)
            {
                clienteMatches = clienteBll.SearchClientes(ClienteParam.telefono, telefonoCliente);
            }
            // Si todos los campos vacíos, devuelve todos los clientes activos.           
            if (searchingField == ClienteParam.empty)
            {
                clienteMatches = clienteBll.SearchClientes(ClienteParam.empty, string.Empty);
            }
            TempData["clientesCoincidentes"] = clienteMatches;
            return RedirectToAction("Index");
        }


        [AutorizarUsuario(permisoId: 9)]
        [HttpGet]
        public ActionResult Details(int id)
        {

            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }

            Cliente cliente = unitOfWork.Cliente.SingleOrDefault(c => c.clienteId == id);
            if (cliente == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDetails(Cliente.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (cliente.activo == 0)
            {
                TempData["mensaje"] = ItemMensaje.ErrorBuscarRegistroEliminado(Cliente.GetNombreModelo(), cliente.clienteId);
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        [AutorizarUsuario(permisoId: 6)]
        [HttpGet]
        public ActionResult Create()
        {
            if (TempData.ContainsKey("mensaje"))
            {
                ViewBag.mensaje = TempData["mensaje"];
            }
            return View();
        }
        
        [AutorizarUsuario(permisoId: 6)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombreCliente,apellido1Cliente,apellido2Cliente,dniCliente,emailCliente,telefonoCliente")] Cliente cliente)
        {
            if (ModelState.IsValid == false || clienteBll.FieldsFormat(cliente) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Cliente.GetNombreModelo());
                return RedirectToAction("Index");

            }
            if (clienteBll.AnyClienteWithDni(cliente.dniCliente))
            {
                ViewBag.mensaje = ItemMensaje.ErrorRegistroDuplicadoCrear(Cliente.GetNombreModelo(), "NIF/NIE", null);
                return View(cliente);
            }
            try
            {
                clienteBll.CreateNewCliente(cliente);
                TempData["mensaje"] = ItemMensaje.SuccessCrear(Cliente.GetNombreModelo(), cliente.dniCliente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ItemMensaje.ErrorExcepcionCrear(Cliente.GetNombreModelo(), ex.GetType().ToString());
                return View(cliente);
            }

        }
      
        [AutorizarUsuario(permisoId: 7)]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Cliente cliente = unitOfWork.Cliente.GetClienteActivo(id);
            if (cliente == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Cliente.GetNombreModelo());
                return RedirectToAction("Index");
            }
            return View(cliente);
        }
      
        [AutorizarUsuario(permisoId: 7)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {

            Cliente cliente = unitOfWork.Cliente.GetClienteActivo(id);
            if (cliente == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Cliente.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (TryUpdateModel(cliente, "", new string[] { "nombreCliente", "apellido1Cliente", "apellido2Cliente", "emailCliente", "telefonoCliente" }) == false)
            {
                ViewBag.mensaje = ItemMensaje.ErrorDatosNoValidosEditar(Cliente.GetNombreModelo());
                return View(cliente);
            }
            if (clienteBll.FieldsFormat(cliente) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Cliente.GetNombreModelo());
                return RedirectToAction("Index");
            }
            try
            {
                clienteBll.UpdateCliente();
                TempData["mensaje"] = ItemMensaje.SuccessEditar(Cliente.GetNombreModelo(), cliente.apellido1Cliente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.mensaje = ItemMensaje.ErrorExcepcionEditar(Cliente.GetNombreModelo(), ex.GetType().ToString());
                return View(cliente);
            }
        }

        /// <summary>
        /// POST: modifica cliente => activo = 0.
        ///  El cliente no se elimina en la BBDD
        /// para poder seguir consultando pólizas del cliente.
        /// </summary>
        /// <param name="clienteId">cliente Id</param>
        /// <return>
        /// Ok => Modifica registro en BBDD y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Details con mensaje de error.
        /// </return>
        [AutorizarUsuario(permisoId: 8)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int clienteId)
        {
            Cliente cliente = unitOfWork.Cliente.GetClienteActivo(clienteId);
            if (cliente == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosDesactivar(Cliente.GetNombreModelo());
                return RedirectToAction("Index");
            }
            // Verifica que el cliente tenga pólizas en vigor.       
            var polizasVigor = context.Poliza.Where(c => c.clienteId == clienteId && c.activo == 1).Select(c => c.polizaId).ToList();
            if (polizasVigor.Any())
            {
                TempData["mensaje"] = ItemMensaje.ErrorPolizaVigorDesactivarCondicionado(Cliente.GetNombreModelo(), polizasVigor);
                return RedirectToAction("Details", new { id = cliente.clienteId });
            }
            else
            {
                try
                {                   
                    clienteBll.DeleteCliente(cliente);
                    TempData["mensaje"] = ItemMensaje.SuccessDesactivar(Cliente.GetNombreModelo(), cliente.apellido1Cliente);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorExcepcionDesactivar(Cliente.GetNombreModelo(), ex.GetType().ToString());
                    return RedirectToAction("Index");
                }
            }
        }
        #endregion
        #region Métodos

        /// <summary>
        /// Comprueba a través de llamada Ajax desde la vista, 
        /// si el NIF / NIE introducido existe en Clientes.  
        /// </summary>
        /// <param name="dni">NIF/NIE cliente</param>
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
            var clienteCoincidente = context.Cliente
                   .Where(c => c.dniCliente == dni).FirstOrDefault();
            if (clienteCoincidente != null)
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
