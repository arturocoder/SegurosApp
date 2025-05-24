using System;
using System.Collections.Generic;

namespace SegurosApp.Api.Models
{
    public class CondicionadoPoliza
    {
        public int CondicionadoPolizaId { get; set; }
        public string TipoCondicionado { get; set; }
        public string Garantias { get; set; }
        public int Activo { get; set; }
        public DateTime? FechaDesactivado { get; set; }

        public ICollection<GestionPoliza> GestionPolizas { get; set; }
    }
}
