using Microsoft.EntityFrameworkCore;
using SegurosApp.Domain.Repositories;
using SegurosApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SegurosApp.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository<Cliente>
    {
        private readonly SegurosDbContext _context;
        public ClienteRepository(SegurosDbContext context)
        {
            _context = context;
        }
        public async Task<Cliente> GetClienteActivoAsync(int clienteId)
        {
            return await _context.Clientes
                .Where(c => c.Activo == 1 && c.ClienteId == clienteId)
                .SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Cliente>> GetClientesActivosWhereAsync(Expression<System.Func<Cliente, bool>> predicate)
        {
            return await _context.Clientes
                .Where(c => c.Activo == 1)
                .Where(predicate)
                .ToListAsync();
        }
    }
}
