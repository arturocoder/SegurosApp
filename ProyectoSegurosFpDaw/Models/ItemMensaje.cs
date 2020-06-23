
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace ProyectoSegurosFpDaw.Models
{
    /// <summary>
    /// Mensajes a enviar  desde Controller a la View.   
    /// <para>Tipo (icono) a mostrar: error , warning ,success, info, question.</para>
    /// <para> Mensaje :  mensaje a mostrar.</para>
    /// </summary>
    public class ItemMensaje
    {
        #region Propiedades / Atributos

        public enum Icono { success = 1, error = 2, info = 3, warning = 4, question = 5 };
        public string Tipo { get; set; }
        public string Mensaje { get; set; }

        #endregion

        #region Constructor/es

        public ItemMensaje()
        {

        }

        public ItemMensaje(string tipo, string mensaje)
        {
            Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
            Mensaje = mensaje ?? throw new ArgumentNullException(nameof(mensaje));
        }
        #endregion

        #region Métodos

        // Mensajes de Success.        

        /// <summary>
        /// Mensaje de Success al crear un registro.
        /// </summary>                
        public static ItemMensaje SuccessCrear(string nombreModelo, string nombreOrNifOrId)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.success.ToString();
            mensaje.Mensaje = nombreModelo + " " + nombreOrNifOrId + " creado/a correctamente.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Success al activar un registro.
        /// </summary> 
        public static ItemMensaje SuccessActivar(string nombreModelo, string nombreOrId)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.success.ToString();
            mensaje.Mensaje = nombreModelo + " " + nombreOrId + " activado/a correctamente.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Success al editar/modificar un registro.
        /// </summary> 
        public static ItemMensaje SuccessEditar(string nombreModelo, string nombreOrId)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.success.ToString();
            mensaje.Mensaje = nombreModelo + " " + nombreOrId + " editado/a correctamente.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Success al cancelar un registro.
        /// </summary> 
        public static ItemMensaje SuccessCancelar(string nombreModelo, string nombreOrId)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.success.ToString();
            mensaje.Mensaje = nombreModelo + " " + nombreOrId + " cancelado/a correctamente.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Success al desactivar un registro.
        /// </summary> 
        public static ItemMensaje SuccessDesactivar(string nombreModelo, string nombreOrId)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.success.ToString();
            mensaje.Mensaje = nombreModelo + " " + nombreOrId + " desactivado/a correctamente.";
            return mensaje;
        }

        // Mensajes de Error Crear.

        /// <summary>
        /// Mensaje de Error al crear un registro. Datos nulos o vacíos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosCrear(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar crear " + nombreModelo + ". \nDatos no válidos o campos vacíos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear un registro. Registro existente en la BDDD.
        /// </summary> 
        public static ItemMensaje ErrorRegistroDuplicadoCrear(string nombreModelo, string campoDuplicado, string[] idsRegistroDuplicado)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            string IdsDuplicados = "";
            if (idsRegistroDuplicado != null && idsRegistroDuplicado.Length > 0)
            {
                foreach (var item in idsRegistroDuplicado)
                {
                    IdsDuplicados = "\n Id :  " + item.ToString(CultureInfo.GetCultureInfo("es-ES")) + IdsDuplicados;
                }
            }
            mensaje.Mensaje = "Error al intentar crear " + nombreModelo +
               ".\nExiste registro/s duplicado/s en la Base de Datos con el mismo " + campoDuplicado + "."
               + IdsDuplicados;
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear una póliza. Nif/Nie con formato incorrecto.
        /// </summary> 
        public static ItemMensaje ErrorNifNoValidoCrearPoliza(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar crear " + nombreModelo + ". \nDebe introducir un NIF/NIE válido para crear una póliza";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear una póliza. Nif/Nie no pertenece a ningún cliente registrado.
        /// </summary> 
        public static ItemMensaje ErrorNifNoExisteCrearPoliza(string nombreModelo, string NIForNIE)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.info.ToString();
            mensaje.Mensaje = "No existe ningún cliente con NIF/NIE " + NIForNIE + ". \nPor favor, cree un nuevo cliente antes de crear la " + nombreModelo;
            return mensaje;
        }

        // Mensajes de error Editar

        /// <summary>
        /// Mensaje de Error al editar/modificar un registro. Datos nulos o vacíos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosEditar(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar editar " + nombreModelo + ".\nDatos no válidos o campos vacíos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al editar/modificar un registro. Registro existente en la BDDD.
        /// </summary>
        /// <param name="campoDuplicado">Campo duplicado (NIF / EMAIL / ...)</param>
        /// <param name="idsRegistroDuplicado">Array de Strings con los Ids de los registros duplicados.</param>        
        public static ItemMensaje ErrorRegistroDuplicadoEditar(string nombreModelo, string campoDuplicado, string[] idsRegistroDuplicado)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            string IdsDuplicados = "";
            if (idsRegistroDuplicado != null && idsRegistroDuplicado.Length > 0)
            {
                foreach (var item in idsRegistroDuplicado)
                {
                    IdsDuplicados = "\n Id :  " + item.ToString(CultureInfo.GetCultureInfo("es-ES")) + IdsDuplicados;
                }
            }
            mensaje.Mensaje = "Error al intentar editar " + nombreModelo +
                ".\nExiste registro/s duplicado/s en la Base de Datos con el mismo " + campoDuplicado + "."
                + IdsDuplicados;
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al editar/desactivar un usuario que sea el único existente con rol administrador. 
        /// </summary> 
        public static ItemMensaje ErrorEditarDesactivarUnicoAdministrador(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.info.ToString();
            mensaje.Mensaje = "No se puede modificar / desactivar el rol administrador del " + nombreModelo + ". \nEs el único usuario con rol administrador.";
            return mensaje;
        }

        // Mensajes de error Cancelar / Desactivar / Activar

        /// <summary>
        /// Mensaje de Error al cancelar un registro. Datos nulos o vacíos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosCancelar(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar cancelar " + nombreModelo + ". \nDatos no válidos o campos vacíos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al cancelar/desactivar un registro. Registro ya cancelado o desactivado.
        /// </summary> 
        public static ItemMensaje ErrorYaCanceladoOrDesactivado(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar cancelar/desactivar " + nombreModelo + ". \n" + nombreModelo + " ya está cancelado/a o desactivado/a.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al desactivar/activar un registro. Datos nulos o vacíos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosDesactivarActivar(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar activar/desactivar " + nombreModelo + ". \nDatos no válidos o campos vacíos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al desactivar un registro. Datos nulos o vacíos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosDesactivar(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar desactivar " + nombreModelo + ". \nDatos no válidos o campos vacíos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al desactivar un condicionado de póliza que tenga pólizas en vigor relacionadas.
        /// </summary>        
        /// <param name="polizasVigor">List de int con los ids de pólizas en vigor</param>        
        public static ItemMensaje ErrorPolizaVigorDesactivarCondicionado(string nombreModelo, List<int> polizasVigor)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            string idsPolizasVigor = "";
            if (polizasVigor != null && polizasVigor.Count > 0)
            {
                foreach (var item in polizasVigor)
                {
                    idsPolizasVigor = "\n Id Póliza :  " + item.ToString(CultureInfo.GetCultureInfo("es-ES")) + idsPolizasVigor;
                }
            }
            mensaje.Mensaje = "Error al intentar desactivar  el " + nombreModelo + ". Existen pólizas en vigor relacionadas con este " + nombreModelo +
                ".\nPor favor, cancele o modifique la/s póliza/s antes de desactivar el " + nombreModelo + "." + idsPolizasVigor;
            return mensaje;
        }

        // Mensajes de Error Buscar / Details

        /// <summary>
        /// Mensaje de Error al buscar un registro. Datos nulos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosBuscar(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar Buscar " + nombreModelo + ". \nDatos no válidos.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al buscar un registro. Registro está desactivado, no se puede visualizar.
        /// </summary> 
        public static ItemMensaje ErrorBuscarRegistroEliminado(string nombreModelo, int idRegistroEliminado)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.info.ToString();
            mensaje.Mensaje = nombreModelo + " con Id : " + idRegistroEliminado + " fue eliminado anteriormente y no se puede visualizar. "
                                + " Si desea recuperarlo, por favor, contacte con el administrador de la base de datos.";
            return mensaje;
        }
        /// <summary>
        /// Mensaje de Error al visualizar Details de un registro. Datos nulos.
        /// </summary> 
        public static ItemMensaje ErrorDatosNoValidosDetails(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar visualizar " + nombreModelo + ". \nDatos no válidos.";
            return mensaje;
        }

        // Errores formatos / validaciones.

        /// <summary>
        /// Mensaje de Error al crear/modificar un registro , no es válido el formato de una matrícula.
        /// </summary> 
        public static ItemMensaje ErrorValidarFormatoMatricula(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al crear / modificar " + nombreModelo + " . La matrícula del vehículo debe tener un formato válido  :" +
                           " \n  Matrícula nueva: 0123-ABC  // Matrícula antigua: AB-0123-CS";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear un registro , matrícula tiene una póliza en vigor.
        /// </summary> 
        public static ItemMensaje ErrorValidarMatriculaDuplicada(string nombreModelo, string matricula)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al crear " + nombreModelo + ". La matrícula " + matricula + " tiene una póliza en vigor.";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear un registro , fecha Inicio es anterior a hoy.
        /// </summary> 
        public static ItemMensaje ErrorFechaInicioMenorHoy(string nombreModelo)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al crear " + nombreModelo + " .Fecha de inicio no puede ser anterior al día de hoy";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear un registro , superado el rango de fecha máximo entre inicio y fin.
        /// </summary> 
        /// <param name="rangoMaximoDias">Rango máximo en días entre Inicio y Fin</param>
        public static ItemMensaje ErrorFechasMaxRangoInicioFin(string nombreModelo, int rangoMaximoDias)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al crear " + nombreModelo + ". El rango máximo entre fecha de Inicio y fecha de fin " +
                           "no puede ser superior a " + rangoMaximoDias;
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error al crear un registro , superado el rango de fecha máximo entre hoy y fin.
        /// </summary> 
        /// <param name="rangoMaximoDias">Rango máximo en días entre hoy y Fin.</param>
        public static ItemMensaje ErrorFechasMaxRangoHoyFin(string nombreModelo, int rangoMaximoDias)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al crear " + nombreModelo + ". El rango máximo entre hoy y fecha de fin " +
                           "no puede ser superior a " + rangoMaximoDias;
            return mensaje;
        }

        // Mensajes Excepciones

        /// <summary>
        /// Mensaje de Error por Excepción al crear un registro. 
        /// </summary>        
        public static ItemMensaje ErrorExcepcionCrear(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar crear " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error por Excepción al editar un registro. 
        /// </summary> 
        public static ItemMensaje ErrorExcepcionEditar(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar editar " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error por Excepción al cancelar un registro. 
        /// </summary> 
        public static ItemMensaje ErrorExcepcionCancelar(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar cancelar " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error por Excepción al desactivar un registro. 
        /// </summary> 
        public static ItemMensaje ErrorExcepcionDesactivar(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar desactivar " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error por Excepción al activar un registro. 
        /// </summary> 
        public static ItemMensaje ErrorExcepcionActivar(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar activar " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        /// <summary>
        /// Mensaje de Error por Excepción al buscar un registro. 
        /// </summary> 
        public static ItemMensaje ErrorExcepcionBuscar(string nombreModelo, string tipoExcepcion)
        {
            ItemMensaje mensaje = new ItemMensaje();
            mensaje.Tipo = Icono.error.ToString();
            mensaje.Mensaje = "Error al intentar buscar " + nombreModelo + ".\nSe ha producido una excepción de tipo: " + tipoExcepcion + " .";
            return mensaje;
        }

        #endregion
    }
}