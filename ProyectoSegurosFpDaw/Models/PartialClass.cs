using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProyectoSegurosFpDaw.Models
{
    [MetadataType(typeof(UsuarioMetadatos))]
    public partial class Usuario
    {
        public static string GetNombreModelo()
        {
            return "Usuario";
        }
    }
    [MetadataType(typeof(ClienteMetadatos))]
    public partial class Cliente
    {
        public static string GetNombreModelo()
        {
            return "Cliente";
        }
    }
    [MetadataType(typeof(CondicionadoPolizaMetadatos))]
    public partial class CondicionadoPoliza
    {
        public static string GetNombreModelo()
        {
            return "Condicionado de Póliza";
        }
    }
    [MetadataType(typeof(RolMetadatos))]
    public partial class Rol
    {
    }
    [MetadataType(typeof(PermisoMetadatos))]
    public partial class Permiso
    {
    }
    [MetadataType(typeof(TipoGestionMetadatos))]
    public partial class TipoGestion
    {
    }
    [MetadataType(typeof(PolizaMetadatos))]
    public partial class Poliza
    {
        public static string GetNombreModelo()
        {
            return "Póliza";
        }
    }
    [MetadataType(typeof(GestionPolizaMetadatos))]
    public partial class GestionPoliza
    {
        public static string GetNombreModelo()
        {
            return "Gestión de Póliza";
        }
    }
}