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
    
    public partial class CondicionadoPoliza
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CondicionadoPoliza()
        {
            this.GestionPoliza = new HashSet<GestionPoliza>();
        }
    
        public int condicionadoPolizaId { get; set; }
        public string tipoCondicionado { get; set; }
        public string garantias { get; set; }
        public int activo { get; set; }
        public Nullable<System.DateTime> fechaDesactivado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GestionPoliza> GestionPoliza { get; set; }
    }
}
