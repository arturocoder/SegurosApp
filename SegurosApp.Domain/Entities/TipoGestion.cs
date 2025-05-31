using System.Collections.Generic;

namespace SegurosApp.Domain.Entities
{
    public class TipoGestion
    {
        public int TipoGestionId { get; set; }
        public string NombreGestion { get; set; }

        public ICollection<GestionPoliza> GestionPolizas { get; set; }
    }
}
