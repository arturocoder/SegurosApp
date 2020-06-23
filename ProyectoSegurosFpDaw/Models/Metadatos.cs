using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProyectoSegurosFpDaw.Models
{
    public class UsuarioMetadatos
    {
        [Display(Name = "Id de Usuario")]
        public int usuarioId;
        [Display(Name = "Nombre")]
        public string nombreUsuario;
        [Display(Name = "Primer Apellido")]
        public string apellido1Usuario;
        [Display(Name = "Segundo Apellido")]
        public string apellido2Usuario;
        [Display(Name = "NIF / NIE")]
        public string dniUsuario;
        [Display(Name = "Email Usuario")]
        public string emailUsuario;
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string password;
        [Display(Name = "Activo")]
        public int activo;
        [Display(Name = "Fecha de Alta")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public System.DateTime fechaAlta;
        [Display(Name = "Fecha de Baja")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> fechaBaja;
        [Display(Name = "Rol")]
        public int rolId;
    }
    public class ClienteMetadatos
    {
        [Display(Name = "Id de Cliente")]        
        public int clienteId;
        [Display(Name = "Nombre")]        
        public string nombreCliente;
        [Display(Name = "Primer Apellido")]        
        public string apellido1Cliente;
        [Display(Name = "Segundo Apellido")]
        public string apellido2Cliente;
        [Display(Name = "NIF / NIE")]
        public string dniCliente;
        [Display(Name = "Email Cliente")]
        public string emailCliente;
        [Display(Name = "Teléfono")]
        public string telefonoCliente;
        [Display(Name = "Activo")]
        public int activo;
        [Display(Name = "Fecha de Baja")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> fechaDesactivado;
    }
    public class CondicionadoPolizaMetadatos
    {
        [Display(Name = "Id de Condicionado")]
        public int condicionadoPolizaId;
        [Display(Name = "Tipo de Condicionado ")]
        public string tipoCondicionado;
        [Display(Name = "Garantías")]
        [DataType(DataType.MultilineText)]
        public string garantias;
        [Display(Name = "Vigente")]
        public int activo;
        [Display(Name = "Fecha de Desactivación")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> fechaDesactivado;
    }
    public class RolMetadatos
    {
        [Display(Name = "Id de Rol")]
        public int rolId;
        [Display(Name = "Rol")]
        public string nombreRol;
        [Display(Name = "Activo")]
        public int activo;
        [Display(Name = "Fecha de Desactivación")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> fechaDesactivado;
    }
    public class PermisoMetadatos
    {
        [Display(Name = "Id de Permiso")]
        public int permisoId;
        [Display(Name = "Permiso")]
        public string nombrePermiso;
        [Display(Name = "Módulo")]
        public int moduloId;

    }
    public class TipoGestionMetadatos
    {
        [Display(Name = "Id de Tipo de Gestión")]
        public int tipoGestionId;
        [Display(Name = "Tipo de Gestión")]
        public string nombreGestion;
    }
    public class PolizaMetadatos
    {
        [Display(Name = "Id de Póliza")]
        public int polizaId;
        [Display(Name = "Estado Póliza")]
        public int activo;
        [Display(Name = "Id de Cliente")]
        public int clienteId;
        [Display(Name = "Fecha de Baja")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public Nullable<System.DateTime> fechaDesactivado;
    }

    public class GestionPolizaMetadatos
    {
        [Display(Name = "Id de Gestión de Póliza")]
        public int gestionPolizaId;
        [Display(Name = "Matrícula")]
        public string matricula;
        [Display(Name = "Marca Vehículo")]
        public string marcaVehiculo;
        [Display(Name = "Modelo")]
        public string modeloVehiculo;
        [Display(Name = "Inicio de Póliza")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public System.DateTime fechaInicio;
        [Display(Name = "Fin de Póliza")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public System.DateTime fechaFin;
        [Display(Name = "Precio")]
        public int precio;
        [Display(Name = "Observaciones")]
        [DataType(DataType.MultilineText)]
        public string observaciones;
        [Display(Name = "Fecha de Gestión")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public System.DateTime fechaGestion;
        [Display(Name = "Condicionado")]
        public int condicionadoPolizaId;
        [Display(Name = "Tipo de Gestión")]
        public int tipoGestionId;
        [Display(Name = "Id de Póliza")]
        public int polizaId;
        [Display(Name = "Id de Usuario")]
        public int usuarioId;
    }

}


