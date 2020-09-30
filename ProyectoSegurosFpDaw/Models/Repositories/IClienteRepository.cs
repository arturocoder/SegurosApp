using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IClienteRepository:IRepository<Cliente>
    {
        Cliente GetClienteActivo(int clienteId);
        IEnumerable<Cliente> GetClientesActivosWhere(Expression<Func<Cliente, bool>> predicate);
    }
}
