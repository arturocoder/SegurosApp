﻿using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }
        public UsuarioRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }


        public Usuario GetAuthenticatedUsuario(string email, string password)
        {
            return ProyectoSegurosContext.Usuario
                       .Where(c => c.emailUsuario == email.Trim() && c.password == password)
                       .FirstOrDefault();
        }
        public Usuario GetUsuarioActivoWhere(Expression<Func<Usuario, bool>> predicate)
        {
            return ProyectoSegurosContext.Usuario                          
                          .Where(c => c.activo == 1)
                          .Where(predicate)
                          .SingleOrDefault();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRoles()
        {

            return ProyectoSegurosContext.Usuario
                      .Include(c => c.Rol)
                      .Where(c => c.activo == 1)
                      .ToList();
        }
        

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesWhere(Expression<Func<Usuario, bool>> predicate)
        {
            return ProyectoSegurosContext.Usuario
                          .Include(c => c.Rol)
                          .Where(c => c.activo==1)
                          .Where(predicate)
                          .ToList();
        }
        public IEnumerable<Usuario> GetUsuariosNoActivosWithRolesWhere(Expression<Func<Usuario, bool>> predicate)
        {
            return ProyectoSegurosContext.Usuario
                          .Include(c => c.Rol)
                          .Where(c => c.activo == 0)
                          .Where(predicate)
                          .ToList();
        }

    }
}