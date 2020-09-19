using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{
    public class PolizaRepository : Repository<Poliza>, IPolizaRepository
    {
        public PolizaRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }
        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }
    }
}