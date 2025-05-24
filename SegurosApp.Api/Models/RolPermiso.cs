namespace SegurosApp.Api.Models
{
    public class RolPermiso
    {
        public int RolPermisoId { get; set; }
        public int PermisoId { get; set; }
        public int RolId { get; set; }

        public Permiso Permiso { get; set; }
        public Rol Rol { get; set; }
    }
}
