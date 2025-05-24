using System;
using System.Collections.Generic;

namespace SegurosApp.Api.Models
{
    public class Poliza
    {
        public int PolizaId { get; set; }
        public int Activo { get; set; }
        public DateTime? FechaDesactivado { get; set; }
        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; }
        public ICollection<GestionPoliza> GestionPolizas { get; set; }
    }
}
