using System;
using System.Collections.Generic;

namespace SegurosApp.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public string Apellido1Cliente { get; set; }
        public string Apellido2Cliente { get; set; }
        public string DniCliente { get; set; }
        public string EmailCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public int Activo { get; set; }
        public DateTime? FechaDesactivado { get; set; }

        public ICollection<Poliza> Polizas { get; set; }
    }
}
