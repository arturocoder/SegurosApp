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

        bool ExistMatriculaInPolizasActivas(string matricula);


        GestionPoliza GetLastGestionPolizaWithClienteBy(int polizaId);     
                
        IEnumerable<GestionPoliza> GetLastGestionesPolizaWithCliente(DateTime fechaInicio, DateTime fechaFinal,int estadoPoliza);
        IEnumerable<GestionPoliza> GetLastGestionesPolizaWithClienteByMatricula(DateTime fechaInicio, DateTime fechaFinal, int estadoPoliza,string matricula);
        IEnumerable<GestionPoliza> GetLastGestionesPolizaWithClienteByDni(DateTime fechaInicio, DateTime fechaFinal, int estadoPoliza,string dni);
        IEnumerable<GestionPoliza> GetLastGestionesPolizaWithClienteByTelefono(DateTime fechaInicio, DateTime fechaFinal, int estadoPoliza,string telefono);


    }
}
