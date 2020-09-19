using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        // Instancia de la BBDD
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
        /// <param name="polizaId">póliza Id</param>
        /// <param name="matricula">matrícula</param>
        /// <param name="dniCliente">NIF/NIE cliente</param>
        /// <param name="telefonoCliente">teléfono cliente</param>
        /// <param name="fechaInicio">fecha Inicio póliza</param>
        /// <param name="fechaFinal">fecha Fin póliza</param>
        /// <param name="estadoPoliza">estado de póliza</param>
        /// <returns>
        /// Hay coincidencias de pólizas => envía una lista GestionPolizas de coincidencias y redirecciona al Index para mostrarlos.
        /// Sin coincidencias => envía una lista Usuario vacía y redirecciona a Index .
        /// Error => redirecciona a Index con mensaje de error.
        /// </returns>
        [HttpGet]
        [AutorizarUsuario(permisoId: 20)]
        public ActionResult BuscarPolizas(int? polizaId, string matricula, string dniCliente, string telefonoCliente, string fechaInicio, string fechaFinal, string estadoPoliza)
        {
            // Validaciones y formato de parámetros.
            int estadoPolizaInt;
            DateTime fechaInicioPoliza = new DateTime();
            DateTime fechaFinalPoliza = new DateTime();
            int contadorParametrosVacios = 0;
            if (polizaId == null) { contadorParametrosVacios++; }
            if (matricula.IsNullOrWhiteSpace() == false) { matricula = matricula.Trim().ToUpperInvariant(); } else { contadorParametrosVacios++; }
            if (dniCliente.IsNullOrWhiteSpace() == false) { dniCliente = dniCliente.Trim().ToUpperInvariant(); } else { contadorParametrosVacios++; }
            if (telefonoCliente.IsNullOrWhiteSpace() == false) { telefonoCliente = telefonoCliente.Trim(); } else { contadorParametrosVacios++; }

            // Los campos fechaInicio ,fechafinal y estadoPoliza son obligatorios.
            if (fechaInicio.IsNullOrWhiteSpace() == false && fechaFinal.IsNullOrWhiteSpace() == false && estadoPoliza.IsNullOrWhiteSpace() == false)
            {
                fechaInicio = fechaInicio.Trim();
                fechaInicioPoliza = DateTime.Parse(fechaInicio, CultureInfo.GetCultureInfo("es-ES"));
                fechaFinal = fechaFinal.Trim();
                fechaFinalPoliza = DateTime.Parse(fechaFinal, CultureInfo.GetCultureInfo("es-ES"));
                estadoPoliza = estadoPoliza.Trim();
                estadoPolizaInt = int.Parse(estadoPoliza, CultureInfo.GetCultureInfo("es-ES"));
            }
            else
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosBuscar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }

            // Búsqueda por parámetros
            // Filtra por los parámetros que no están vacíos , 
            // busca coincidencias en la BBDD 
            // Parámetro 
            // + estado Póliza ( 0=> no activa , 1 => activa ,  2=> todos) 
            // + fecha de Alta de la póliza (rango de fechas comprendido entre FechaInicio/FechaFinal)
            // Crea una lista de gestion pólizas coincidentes (la última creada de cada póliza coincidente )
            // Envía la lista a la acción Index.

            var ultimaGestionLista = new List<GestionPoliza>();

            try
            {
                // Fecha Alta + Estado  (resto de campos vacíos).
                if (contadorParametrosVacios == 4)
                {
                    // Estado Póliza Todos. 
                    if (estadoPolizaInt == 2)
                    {
                        // Obtiene el id de las pólizas que coinciden con el rango de fecha de Alta.
                        var polizasCoincidentes =
                             from gestiones in context.GestionPoliza
                             join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                             where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                             select new { Poliza = polizas.polizaId };

                        // Si hay resultados coincidentes
                        if (polizasCoincidentes.Any())
                        {
                            // Recorre la query (obviando las pólizas repetidas (distinct)) 
                            foreach (var item in polizasCoincidentes.Distinct())
                            {
                                // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º)
                                var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                    .Where(c => c.polizaId == item.Poliza)
                                    .OrderByDescending(c => c.gestionPolizaId)
                                    .FirstOrDefault();

                                // Añade a la lista 
                                ultimaGestionLista.Add(ultimaGestion);
                            }
                            TempData["polizasCoincidentes"] = ultimaGestionLista;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        // Obtiene el id de las pólizas que coinciden con 
                        // el rango de fecha de Alta + estado Póliza (activo/No activo).
                        var polizasCoincidentes =
                             from gestiones in context.GestionPoliza
                             join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                             where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                && polizas.activo == estadoPolizaInt
                             select new { Poliza = polizas.polizaId };

                        // Si hay resultados coincidentes.
                        if (polizasCoincidentes.Any())
                        {
                            // Recorre la query (obviando las pólizas repetidas (distinct)). 
                            foreach (var item in polizasCoincidentes.Distinct())
                            {
                                // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                    .Where(c => c.polizaId == item.Poliza)
                                    .OrderByDescending(c => c.gestionPolizaId)
                                    .FirstOrDefault();

                                // Añade a la lista.
                                ultimaGestionLista.Add(ultimaGestion);
                            }
                            TempData["polizasCoincidentes"] = ultimaGestionLista;
                            return RedirectToAction("Index");
                        }
                    }
                }
                // Si se ha introducido datos solo en 1 de los campos.          
                else if (contadorParametrosVacios == 3)
                {
                    // Póliza Id 
                    if (polizaId != null)
                    {
                        var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                    .Where(c => c.polizaId == polizaId)
                                    .OrderByDescending(c => c.gestionPolizaId)
                                    .FirstOrDefault();
                        if (ultimaGestion != null)
                        {
                            ultimaGestionLista.Add(ultimaGestion);
                            TempData["polizasCoincidentes"] = ultimaGestionLista;
                            return RedirectToAction("Index");
                        }
                    }
                    // Fecha Alta + Estado + Matrícula que aparezca en cualquiera de sus gestionPoliza.
                    else if (matricula.IsEmpty() == false)
                    {
                        // Estado Póliza Todos 
                        if (estadoPolizaInt == 2)
                        {
                            // Obtiene el id de las pólizas que coinciden con el rango de fecha de Alta + matrícula
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && gestiones.matricula == matricula
                                 select new { Poliza = polizas.polizaId };

                            // Si hay resultados coincidentes.
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)). 
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();

                                    // Añade a la lista 
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            // Obtiene id de las pólizas que coinciden con 
                            // el rango de fecha de Alta + estado Póliza (activo/No activo) + matrícula.                     
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && polizas.activo == estadoPolizaInt && gestiones.matricula == matricula
                                 select new { Poliza = polizas.polizaId };

                            // Si hay resultados coincidentes
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)).
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º)
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();
                                    // Añade a la lista.
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    // Fecha Alta + Estado + NIF / NIE.
                    else if (dniCliente.IsEmpty() == false)
                    {
                        // Estado Póliza Todos.
                        if (estadoPolizaInt == 2)
                        {
                            // Obtiene id de las pólizas que coinciden con el rango de fecha de Alta + NIF / NIE.
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && polizas.Cliente.dniCliente == dniCliente
                                 select new { Poliza = polizas.polizaId };

                            // Si hay resultados coincidentes.
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)).
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();

                                    // Añade a la lista.
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            // Obtiene el id de las pólizas que coinciden con 
                            // el rango de fecha de Alta + estado Póliza (activo/No activo) + matrícula.                  
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && polizas.activo == estadoPolizaInt && polizas.Cliente.dniCliente == dniCliente
                                 select new { Poliza = polizas.polizaId };
                            // Si hay resultados coincidentes.
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)).
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();
                                    // Añade a la lista .
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    // Fecha Alta + Estado + Teléfono.
                    else if (telefonoCliente.IsEmpty() == false)
                    {
                        // Estado Póliza Todos.
                        if (estadoPolizaInt == 2)
                        {
                            // Obtiene el id de las pólizas que coinciden con el rango de fecha de Alta + teléfono.
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && polizas.Cliente.telefonoCliente == telefonoCliente
                                 select new { Poliza = polizas.polizaId };
                            // Si hay resultados coincidentes.
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)).
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();
                                    // Añade a la lista.
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            // Obtiene el id de las pólizas que coinciden
                            // con el rango de fecha de Alta + estado Póliza (activo/No activo) + matrícula.                    
                            var polizasCoincidentes =
                                 from gestiones in context.GestionPoliza
                                 join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
                                 where gestiones.fechaInicio < fechaFinalPoliza && gestiones.fechaInicio > fechaInicioPoliza
                                    && polizas.activo == estadoPolizaInt && polizas.Cliente.telefonoCliente == telefonoCliente
                                 select new { Poliza = polizas.polizaId };

                            // Si hay resultados coincidentes.
                            if (polizasCoincidentes.Any())
                            {
                                // Recorre la query (obviando las pólizas repetidas (distinct)).
                                foreach (var item in polizasCoincidentes.Distinct())
                                {
                                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º).
                                    var ultimaGestion = context.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                                        .Where(c => c.polizaId == item.Poliza)
                                        .OrderByDescending(c => c.gestionPolizaId)
                                        .FirstOrDefault();
                                    // Añade a la lista 
                                    ultimaGestionLista.Add(ultimaGestion);
                                }
                                TempData["polizasCoincidentes"] = ultimaGestionLista;
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                else
                {
                    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosBuscar(Poliza.GetNombreModelo());
                    return RedirectToAction("Index");
                }
                // Si no hay ningún resultado coincidente, devuelve una lista vacía.
                TempData["polizasCoincidentes"] = ultimaGestionLista;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionBuscar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }



        /// <summary>
        /// GET: Muestra la información de la última gestionPoliza de una póliza.
        /// </summary>
        /// <param name="id">gestionPoliza Id</param>
        /// <returns>Vista de la información del usuario</returns>
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
            ViewBag.gestionPolizaIdLast = historicoPoliza.LastOrDefault().gestionPolizaId;
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
        /// <param name="gestionPoliza"> gestión póliza con: matricula,marca,modelo,fecha Inicio, fecha fin
        /// precio, observaciones y condicionado</param>
        /// <param name="clienteId"> id de cliente</param>
        /// Ok => Guarda registro en BBDD (póliza y gestión póliza) y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Create con mensaje de error.
        [AutorizarUsuario(permisoId: 12)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "matricula,marcaVehiculo,modeloVehiculo,fechaInicio,fechaFin,precio,observaciones,condicionadoPolizaId")] GestionPoliza gestionPoliza, string clienteId)
        {
            if (ModelState.IsValid == false || gestionPolizaBLL.FieldsFormat(gestionPoliza, clienteId) == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            // Validaciones y formato de parámetros
            bool success = Int32.TryParse(clienteId.Trim(), out int clienteID);
            if (success == false)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            var dni = context.Cliente.Where(c => c.clienteId == clienteID).Select(c => c.dniCliente).FirstOrDefault();
            if (dni == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }

            // Creación de la póliza.
            try
            {
                //if (gestionPoliza.matricula.IsNullOrWhiteSpace() || gestionPoliza.marcaVehiculo.IsNullOrWhiteSpace()
                //    || gestionPoliza.modeloVehiculo.IsNullOrWhiteSpace() || gestionPoliza.observaciones.IsNullOrWhiteSpace()
                //    || gestionPoliza.fechaInicio == null || gestionPoliza.fechaFin == null)
                //{
                //    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                //    return RedirectToAction("Index");
                //}
                //gestionPoliza.matricula = gestionPoliza.matricula.Trim().ToUpperInvariant();
                //gestionPoliza.marcaVehiculo = gestionPoliza.marcaVehiculo.Trim().ToUpperInvariant();
                //gestionPoliza.modeloVehiculo = gestionPoliza.modeloVehiculo.Trim().ToUpperInvariant();
                //gestionPoliza.observaciones = gestionPoliza.observaciones.Trim();

                if (ValidarFormatoMatricula(gestionPoliza.matricula) == false)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorValidarFormatoMatricula(Poliza.GetNombreModelo());
                    return RedirectToAction("Create", new { clienteDni = dni });
                }
                if (VerificarMatriculaDuplicada(gestionPoliza.matricula) == true)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorValidarMatriculaDuplicada(Poliza.GetNombreModelo(), gestionPoliza.matricula);
                    return RedirectToAction("Index");
                }
                var usuario = GetUsuarioActual();
                if (usuario == null)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCrear(Poliza.GetNombreModelo());
                    return RedirectToAction("Index");
                }

                DateTime hoyFecha = DateTime.Today;
                if (gestionPoliza.fechaInicio < hoyFecha)
                {
                    TempData["mensaje"] = ItemMensaje.ErrorFechaInicioMenorHoy(Poliza.GetNombreModelo());
                    return RedirectToAction("Create", new { clienteDni = dni });
                }
                if (gestionPoliza.fechaFin > gestionPoliza.fechaInicio.AddYears(1))
                {
                    TempData["mensaje"] = ItemMensaje.ErrorFechasMaxRangoInicioFin(Poliza.GetNombreModelo(), 365);
                    return RedirectToAction("Create", new { clienteDni = dni });
                }
                if (gestionPoliza.fechaFin > hoyFecha.AddYears(1).AddMonths(6))
                {
                    TempData["mensaje"] = ItemMensaje.ErrorFechasMaxRangoHoyFin(Poliza.GetNombreModelo(), 547);
                    return RedirectToAction("Create", new { clienteDni = dni });
                }

                // Asigna valores a la gestión póliza.
                gestionPoliza.usuarioId = usuario.usuarioId;
                DateTime hoy = DateTime.Now;
                gestionPoliza.fechaGestion = hoy;

                // Tipo de gestión
                // 1 => ALTA 
                gestionPoliza.tipoGestionId = 1;
                var poliza = new Poliza();

                // poliza.activo =>  
                // -1 =>  estado temporal mientras se procesa la generación de póliza,
                // para poder recuperar el id de póliza si al generar el primer gestionPoliza se produce algún error.
                poliza.activo = -1;
                poliza.clienteId = clienteID;

                //Crea el registro en la BBDD.
                context.Poliza.Add(poliza);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Comprueba que se haya creado una póliza.
                var polizaCreada = context.Poliza.Where(c => c.clienteId == clienteID && c.activo == -1).FirstOrDefault();
                //Si se ha creado, elimina póliza y guarda cambios.
                if (polizaCreada != null)
                {
                    context.Poliza.Remove(polizaCreada);
                    context.SaveChanges();
                }
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Create", new { clienteDni = dni });

            }
            // Creación de la gestión póliza
            try
            {
                // Recupera la póliza creada.
                var polizaIdCreada = context.Poliza.Where(c => c.clienteId == clienteID && c.activo == -1).Select(s => s.polizaId).FirstOrDefault();
                // Crea la gestiónPóliza Inicial de Alta.
                gestionPoliza.polizaId = polizaIdCreada;
                context.GestionPoliza.Add(gestionPoliza);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Comprueba que se haya creado una póliza.
                var polizaCreada = context.Poliza.Where(c => c.clienteId == clienteID && c.activo == -1).FirstOrDefault();
                // Si se ha creado, elimina póliza y guarda cambios.
                if (polizaCreada != null)
                {
                    context.Poliza.Remove(polizaCreada);
                    context.SaveChanges();
                }
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Create", new { clienteDni = dni });
            }
            try
            {

                // Recupera la póliza creada y cambia su estado a activo =  1                                      
                var polizaCreada = context.Poliza.Where(c => c.clienteId == clienteID && c.activo == -1).FirstOrDefault();
                polizaCreada.activo = 1;

                // Actualiza póliza en BBDD.
                context.Entry(polizaCreada).State = EntityState.Modified;
                context.SaveChanges();
                TempData["mensaje"] = ItemMensaje.SuccessCrear(Poliza.GetNombreModelo(), polizaCreada.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Comprueba que se haya creado una póliza 
                var polizaCreada = context.Poliza.Where(c => c.clienteId == clienteID && c.activo == -1).FirstOrDefault();
                // si se ha creado póliza 
                if (polizaCreada != null)
                {
                    // Comprueba que se haya creado la gestiónPóliza , y la elimina
                    var gestionPolizaCreada = context.GestionPoliza.Where(c => c.polizaId == polizaCreada.polizaId).FirstOrDefault();
                    if (gestionPolizaCreada != null)
                    {
                        context.GestionPoliza.Remove(gestionPolizaCreada);
                    }

                    // Elimina póliza y guarda cambios.
                    context.Poliza.Remove(polizaCreada);
                    context.SaveChanges();
                }
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCrear(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Create", new { clienteDni = dni });
            }


        }

        /// <summary>
        /// GET: formulario para modificar una póliza => crea una nueva gestión póliza. 
        /// </summary>
        /// <param name="id">polizaId</param>
        /// <returns>Vista con formulario para modificar una póliza (gestión póliza)</returns>
        [AutorizarUsuario(permisoId: 13)]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            // Recupera la última gestión de la póliza. 
            GestionPoliza gestionPoliza = context.GestionPoliza
                .Include(c => c.Poliza.Cliente)
                .Include(c => c.CondicionadoPoliza)
                .Include(c => c.TipoGestion)
                .Where(c => c.polizaId == id)
                .OrderByDescending(c => c.gestionPolizaId)
                .FirstOrDefault();
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            ViewBag.condicionadoPolizaId = new SelectList(context.CondicionadoPoliza
                .Where(c => c.activo == 1), "condicionadoPolizaId", "tipoCondicionado", gestionPoliza.condicionadoPolizaId);
            return View(gestionPoliza);

        }

        /// <summary>
        /// Modifica una póliza =>  crea una nueva gestión póliza.
        /// </summary>
        /// <param name="id">polizaId</param>
        /// <returns>
        ///  Ok => crea nueva gestionPoliza  en BBDD asociada a la póliza y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index / Details con mensaje de error.
        /// </returns>
        [AutorizarUsuario(permisoId: 13)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }

            // Recupera la última gestión póliza de la  póliza (orden descendente => selecciona primer registro).
            GestionPoliza gestionPoliza = context.GestionPoliza.Where(c => c.polizaId == id).OrderByDescending(c => c.gestionPolizaId).FirstOrDefault();
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }

            var matriculaEstadoPrevio = gestionPoliza.matricula;
            try
            {
                // Intenta actualizar la gestión póliza con los datos enviados en el formulario post 
                if (TryUpdateModel(gestionPoliza, "", new string[] { "condicionadoPolizaId", "precio", "observaciones", "matricula", "marcaVehiculo", "modeloVehiculo" }))
                {

                    // Validaciones y formato de parámetros.
                    if (gestionPoliza.matricula.IsNullOrWhiteSpace() || gestionPoliza.marcaVehiculo.IsNullOrWhiteSpace() || gestionPoliza.modeloVehiculo.IsNullOrWhiteSpace() || gestionPoliza.observaciones.IsNullOrWhiteSpace())
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                        return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });

                    }
                    gestionPoliza.matricula = gestionPoliza.matricula.Trim().ToUpperInvariant();
                    gestionPoliza.marcaVehiculo = gestionPoliza.marcaVehiculo.Trim().ToUpperInvariant();
                    gestionPoliza.modeloVehiculo = gestionPoliza.modeloVehiculo.Trim().ToUpperInvariant();
                    gestionPoliza.observaciones = gestionPoliza.observaciones.Trim();

                    if (ValidarFormatoMatricula(gestionPoliza.matricula) == false)
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorValidarFormatoMatricula(Poliza.GetNombreModelo());
                        return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
                    }

                    // Si se ha modificado la matrícula                     
                    if (matriculaEstadoPrevio != gestionPoliza.matricula)
                    {
                        if (VerificarMatriculaDuplicada(gestionPoliza.matricula) == true)
                        {
                            TempData["mensaje"] = ItemMensaje.ErrorValidarMatriculaDuplicada(Poliza.GetNombreModelo(), gestionPoliza.matricula);
                            return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
                        }
                    }
                    var usuario = GetUsuarioActual();
                    if (usuario == null)
                    {
                        TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                        return RedirectToAction("Index");
                    }

                    //Asignación de valores
                    gestionPoliza.usuarioId = usuario.usuarioId;
                    DateTime hoy = DateTime.Now;
                    gestionPoliza.fechaGestion = hoy;

                    // Tipo de gestión
                    // 3 => MODIFICACIÓN 
                    gestionPoliza.tipoGestionId = 3;

                    //Guarda nueva gestión Póliza en la BBDD.
                    context.GestionPoliza.Add(gestionPoliza);
                    context.SaveChanges();
                    TempData["mensaje"] = ItemMensaje.SuccessEditar(Poliza.GetNombreModelo(), gestionPoliza.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                    return RedirectToAction("Index");
                }
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosEditar(Poliza.GetNombreModelo());
                return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionEditar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Details", new { id = gestionPoliza.gestionPolizaId });
            }
        }


        /// <summary>
        /// POST : cancela una póliza, modificando su estado activo a 0, 
        /// y creando una nueva gestión póliza modificando su fecha de fin.
        /// </summary>
        /// <param name="gestionPolizaId">gestionPoliza Id</param>
        /// <param name="motivoClx">motivo de cancelación</param>
        /// Ok => Modifica póliza en BBDD, crea nueva gestiónPóliza, y redirecciona a Index con mensaje de success.
        /// Error => redirecciona a Index .
        [AutorizarUsuario(permisoId: 14)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int gestionPolizaId, string motivoClx)
        {
            GestionPoliza gestionPoliza = context.GestionPoliza.Find(gestionPolizaId);
            if (gestionPoliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCancelar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            Poliza poliza = context.Poliza.Find(gestionPoliza.polizaId);
            if (poliza == null)
            {
                TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCancelar(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            if (poliza.activo == 0)
            {
                TempData["mensaje"] = ItemMensaje.ErrorYaCanceladoOrDesactivado(Poliza.GetNombreModelo());
                return RedirectToAction("Index");
            }
            try
            {
                var usuario = GetUsuarioActual();
                gestionPoliza.usuarioId = usuario.usuarioId;
                DateTime hoy = DateTime.Now;
                gestionPoliza.fechaGestion = hoy;
                DateTime hoyFecha = DateTime.Today;

                // Validaciones

                // Si es una póliza con fecha de inicio futura ,asigna mismo valor a fecha Inicio /Fin.
                if (gestionPoliza.fechaInicio > hoyFecha)
                {
                    gestionPoliza.fechaFin = gestionPoliza.fechaInicio;
                }
                else
                {
                    gestionPoliza.fechaFin = hoyFecha;

                }

                // Tipo de gestión:
                // 2 => BAJA 
                gestionPoliza.tipoGestionId = 2;
                if (motivoClx.IsNullOrWhiteSpace())
                {
                    TempData["mensaje"] = ItemMensaje.ErrorDatosNoValidosCancelar(Poliza.GetNombreModelo());
                    return RedirectToAction("Index");
                }
                else
                {
                    gestionPoliza.observaciones = "Póliza cancelada por usuario : " + usuario.emailUsuario + ". \nMotivo : " + motivoClx.Trim();
                }

                //Crea la nueva gestión póliza.
                context.GestionPoliza.Add(gestionPoliza);

                //Desactiva la póliza (activo = 0)
                //Guarda la fecha de hoy como fecha Desactivado.              
                poliza.fechaDesactivado = hoy;
                poliza.activo = 0;

                // Actualiza y guarda cambios en BBDD.
                context.Entry(poliza).State = EntityState.Modified;
                context.SaveChanges();
                TempData["mensaje"] = ItemMensaje.SuccessCancelar(Poliza.GetNombreModelo(), poliza.polizaId.ToString(CultureInfo.GetCultureInfo("es-ES")));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Si no se ha podido dar de baja la póliza
                // Comprueba que se haya cambiado el estado activo
                var polizaModificada = context.Poliza.Find(poliza.polizaId);
                if (polizaModificada.activo == 0)
                {
                    polizaModificada.activo = 1;
                    context.Entry(polizaModificada).State = EntityState.Modified;

                }
                // Comprueba que se haya creado una gestión Póliza con estado baja y la elimina
                var gestionPolizaModificada = context.GestionPoliza
                    .Where(c => c.polizaId == poliza.polizaId && c.tipoGestionId == 2).FirstOrDefault();
                if (gestionPolizaModificada != null)
                {
                    context.GestionPoliza.Remove(gestionPolizaModificada);
                }

                //Guarda cambios en BBDD
                context.SaveChanges();
                TempData["mensaje"] = ItemMensaje.ErrorExcepcionCancelar(Poliza.GetNombreModelo(), ex.GetType().ToString());
                return RedirectToAction("Index");
            }
        }
        #endregion
        #region Métodos


        /// <summary>
        /// Verifica que la matrícula ya esté dada de alta en una póliza 
        /// que esté en vigor o con fecha de inicio futura (poliza activa=1)
        /// </summary>
        /// <param name="matricula">matricula</param>
        /// <returns> 
        /// true => si está duplicada.
        /// false => si no está duplicada .
        /// </returns>
        private bool VerificarMatriculaDuplicada(string matricula)
        {
            var matriculaComprobar = matricula.Trim().ToUpperInvariant();
            //Verificar las gestiones polizas que tengan esa matricula  , y que la póliza esté activa                      
            var query =
               from gestiones in context.GestionPoliza
               join polizas in context.Poliza on gestiones.polizaId equals polizas.polizaId
               where gestiones.matricula == matriculaComprobar && polizas.activo == 1
               select new { GestionPoliza = gestiones };

            var hayDatos = query.Count();
            if (hayDatos > 0) { return true; } else { return false; }
        }

        /// <summary>
        /// Obtiene el Usuario de la Session actual (usuario logado).
        /// </summary>
        /// <returns>
        /// usuario
        /// </returns>
        private Usuario GetUsuarioActual()
        {
            Usuario oUsuario = (Usuario)Session["user"];
            return oUsuario;
        }

        /// <summary>
        /// Validación de formato de la matrícula mediante expresión regular .
        ///<para>
        ///Obtenido de : https://www.laps4.com/comunidad/threads/necesito-funcion-javascript-para-validar-matriculas.186497/
        ///</para>
        /// </summary>
        /// <param name="matricula">matrícula</param>
        /// <returns>
        /// true => matricula correcta
        /// false => matricula incorrecta        
        /// </returns>
        private bool ValidarFormatoMatricula(string matricula)
        {

            // Matrícula nueva: 0123-ABC
            // Matrícula antigua: AB-0123-CS
            string pattern = @"(\d{4}-[\D\w]{3}|[\D\w]{1,2}-\d{4}-[\D\\w]{2})";
            Regex rgx = new Regex(pattern);
            if (rgx.IsMatch(matricula))
            {
                return true;
            }
            else
            {
                return false;
            }
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
