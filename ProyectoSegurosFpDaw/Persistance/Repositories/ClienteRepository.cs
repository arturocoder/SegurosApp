using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }
        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }
        public Cliente GetClienteActivo(int clienteId)
        {
            return ProyectoSegurosContext.Cliente
                           .Where(c => c.activo == 1 && c.clienteId == clienteId)
                           .SingleOrDefault();
        }
        public IEnumerable<Cliente> GetClientesActivosWhere(Expression<Func<Cliente, bool>> predicate)
        {
            return ProyectoSegurosContext.Cliente                          
                          .Where(c => c.activo == 1)
                          .Where(predicate)
                          .ToList();
        }
    }
}