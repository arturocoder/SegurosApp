using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IUnitOfWork 
    {
        IRolRepository Rol { get; }
        ICondicionadoPolizaRepository CondicionadoPoliza { get; }
        IUsuarioRepository Usuario { get; }
        IClienteRepository Cliente { get; }
        IGestionPolizaRepository GestionPoliza { get; }
        IPolizaRepository Poliza { get; }

        int SaveChanges();
        
    }
}
