using System.Collections.Generic;
using System.Threading.Tasks;
using SegurosApp.Api.Models;
using System.Linq.Expressions;

namespace SegurosApp.Api.Models.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente> GetClienteActivoAsync(int clienteId);
        Task<IEnumerable<Cliente>> GetClientesActivosWhereAsync(Expression<System.Func<Cliente, bool>> predicate);
    }
}