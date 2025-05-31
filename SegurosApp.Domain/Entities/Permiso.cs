using System.Collections.Generic;

namespace SegurosApp.Domain.Entities
{
    public class Permiso
    {
        public int PermisoId { get; set; }
        public string NombrePermiso { get; set; }
        public int ModuloId { get; set; }

        public Modulo Modulo { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}
