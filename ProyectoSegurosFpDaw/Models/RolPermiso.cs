//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoSegurosFpDaw.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RolPermiso
    {
        public int rolPermisoId { get; set; }
        public int permisoId { get; set; }
        public int rolId { get; set; }
    
        public virtual Permiso Permiso { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
