using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using ProyectoSegurosFpDaw.Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProyectoSegurosDbEntities _context;

        public UnitOfWork(ProyectoSegurosDbEntities context)
        {
            _context = context;
            Roles = new RolRepository(_context);
            CondicionadoPoliza = new CondicionadoPolizaRepository(_context);
            
        }

        public IRolRepository Roles { get; private set; }
        public ICondicionadoPolizaRepository CondicionadoPoliza { get; set; }

        public int Save()
        {
            return _context.SaveChanges();
        }
        
        
    }
}