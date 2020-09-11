using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IRolRepository:IRepository<Rol>
    {
        IEnumerable<Rol> GetRolesWithRolesPermisos();
        

    }
}
