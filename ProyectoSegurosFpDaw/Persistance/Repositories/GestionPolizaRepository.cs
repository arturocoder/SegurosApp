using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{

    public class GestionPolizaRepository : Repository<GestionPoliza>, IGestionPolizaRepository
    {
        public GestionPolizaRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }

        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }


        public GestionPoliza GetGestionPolizaWithClienteCondicionadoTipoGestion(int gestionPolizaId)
        {
            return ProyectoSegurosContext.GestionPoliza
                .Include(c => c.Poliza.Cliente)
                .Include(c => c.CondicionadoPoliza)
                .Include(c => c.TipoGestion)
                .Where(c => c.gestionPolizaId == gestionPolizaId).FirstOrDefault();
        }
        public IEnumerable<GestionPoliza> GetGestionesPolizaWithClienteCondicionadoTipoGestionWhere(Expression<Func<GestionPoliza, bool>> predicate)
        {
            return ProyectoSegurosContext.GestionPoliza
                .Include(c => c.Poliza.Cliente)
                .Include(c => c.CondicionadoPoliza)
                .Include(c => c.TipoGestion)
                .Where(predicate)
                .ToList();
                
        }
    }
}