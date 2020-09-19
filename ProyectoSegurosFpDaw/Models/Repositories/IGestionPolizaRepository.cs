using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IGestionPolizaRepository:IRepository<GestionPoliza>
    {
        GestionPoliza GetGestionPolizaWithClienteCondicionadoTipoGestion(int gestionPolizaId);

        IEnumerable<GestionPoliza> GetGestionesPolizaWithClienteCondicionadoTipoGestionWhere(Expression<Func<GestionPoliza, bool>> predicate);


    }
}
