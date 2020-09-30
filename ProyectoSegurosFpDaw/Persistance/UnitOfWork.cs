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
            Cliente = new ClienteRepository(_context);
            GestionPoliza = new GestionPolizaRepository(_context);
            Poliza = new PolizaRepository(_context);
        }

        public IRolRepository Rol { get; private set; }
        public ICondicionadoPolizaRepository CondicionadoPoliza { get; set; }
        public IUsuarioRepository Usuario { get; set; }
        public IClienteRepository Cliente { get; set; }
        public IGestionPolizaRepository GestionPoliza { get; set; }
        public IPolizaRepository Poliza { get; set; }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}