using System.Collections.Generic;

namespace SegurosApp.Api.Models
{
    public class Modulo
    {
        public int ModuloId { get; set; }
        public string NombreModulo { get; set; }

        public ICollection<Permiso> Permisos { get; set; }
    }
}
