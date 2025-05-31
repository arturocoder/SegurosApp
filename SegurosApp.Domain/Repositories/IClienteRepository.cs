using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SegurosApp.Domain.Repositories
{
    public interface IClienteRepository<TCliente>
    {
        Task<TCliente> GetClienteActivoAsync(int clienteId);
        Task<IEnumerable<TCliente>> GetClientesActivosWhereAsync(Expression<System.Func<TCliente, bool>> predicate);
    }
}
