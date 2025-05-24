using System;
using System.Collections.Generic;

namespace SegurosApp.Api.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public int Activo { get; set; }
        public DateTime? FechaDesactivado { get; set; }

        public ICollection<RolPermiso> RolPermisos { get; set; }
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
