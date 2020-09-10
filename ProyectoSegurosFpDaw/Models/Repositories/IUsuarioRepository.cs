using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IUsuarioRepository:IRepository<Usuario>
    {
        Usuario GetAuthenticatedUsuario(string email, string password);
        IEnumerable<Usuario> GetUsuariosActivosWithRoles();
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByRol(int rolId);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombre(string nombre);

        IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreRol(string nombre,int rolId);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByApellido(string apellido1);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByApellidoRol(string apellido1,int rolId);


        IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreApellido(string nombre,string apellido1);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByNombreApellidoRol(string nombre, string apellido1,int rolId);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByDni(string dni);
        IEnumerable<Usuario> GetUsuariosNoActivosWithRolesByDni(string dni);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByDniRol(string dni,int rolId);
        IEnumerable<Usuario> GetUsuariosActivosWithRolesByEmail(string email);
        IEnumerable<Usuario> GetUsuariosNoActivosWithRolesByEmail(string email);

        IEnumerable<Usuario> GetUsuariosActivosWithRolesByEmailRol(string email,int rolId);










    }
}
