using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public IEnumerable<Usuario> GetUsuariosActivosWithRoles()
        {

            return ProyectoSegurosContext.Usuario
                      .Include(c => c.Rol)
                      .Where(c => c.activo == 1)
                      .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByRol(int rolId)
        {
            return ProyectoSegurosContext.Usuario
                        .Include(c => c.Rol)
                        .Where(c => c.activo == 1 && c.rolId == rolId)
                        .ToList();
        }
        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombre(string nombre)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.nombreUsuario == nombre)
                               .ToList();
        }
        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreRol(string nombre, int rolId)
        {
            return ProyectoSegurosContext.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.nombreUsuario == nombre && c.rolId == rolId).ToList();
        }
        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByApellido(string apellido1)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.apellido1Usuario == apellido1)
                               .ToList();
        }
        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByApellidoRol(string apellido1, int rolId)
        {
            return ProyectoSegurosContext.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.apellido1Usuario == apellido1 && c.rolId == rolId).ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreApellido(string nombre, string apellido1)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.nombreUsuario == nombre && c.apellido1Usuario == apellido1)
                               .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreApellidoRol(string nombre, string apellido1, int rolId)
        {
            return ProyectoSegurosContext.Usuario
                            .Include(c => c.Rol)
                            .Where(c => c.activo == 1 && c.nombreUsuario == nombre && c.apellido1Usuario == apellido1 && c.rolId == rolId)
                            .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByDni(string dni)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.dniUsuario == dni)
                               .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosNoActivosWithRolesByDni(string dni)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 0 && c.dniUsuario == dni)
                               .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByDniRol(string dni, int rolId)
        {
            return ProyectoSegurosContext.Usuario
                            .Include(c => c.Rol)
                            .Where(c => c.activo == 1 && c.dniUsuario == dni && c.rolId == rolId).ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByEmail(string email)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 1 && c.emailUsuario == email)
                               .ToList();
        }
        public IEnumerable<Usuario> GetUsuariosNoActivosWithRolesByEmail(string email)
        {
            return ProyectoSegurosContext.Usuario
                               .Include(c => c.Rol)
                               .Where(c => c.activo == 0 && c.emailUsuario == email)
                               .ToList();
        }

        public IEnumerable<Usuario> GetUsuariosActivosWithRolesByEmailRol(string email, int rolId)
        {
            return ProyectoSegurosContext.Usuario
                           .Include(c => c.Rol)
                           .Where(c => c.activo == 1 && c.emailUsuario == email && c.rolId == rolId).ToList();
        }
    }
}