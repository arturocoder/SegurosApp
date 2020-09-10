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
            Rol = new RolRepository(_context);
            CondicionadoPoliza = new CondicionadoPolizaRepository(_context);
            Usuario = new UsuarioRepository(_context);
            
            
        }

        public IRolRepository Rol { get; private set; }
        public ICondicionadoPolizaRepository CondicionadoPoliza { get; set; }
        public IUsuarioRepository Usuario { get; set; }


        public int Save()
        {
            return _context.SaveChanges();
        }
        
        
    }
}