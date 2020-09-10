using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{
    public class RolRepository : Repository<Rol>, IRolRepository
    {
        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }


        public RolRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }

        public IEnumerable<Rol> GetRolesWithRolesPermisos()
        {
             return ProyectoSegurosContext.Rol
                .Include(r => r.RolPermiso)
                .ToList();
        }

    }
}