using System;
using System.Collections.Generic;

namespace SegurosApp.Api.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Apellido1Usuario { get; set; }
        public string Apellido2Usuario { get; set; }
        public string DniUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public string Password { get; set; }
        public int Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int RolId { get; set; }

        public ICollection<GestionPoliza> GestionPolizas { get; set; }
        public Rol Rol { get; set; }
    }
}
