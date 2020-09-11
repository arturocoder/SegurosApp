using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoSegurosFpDaw.Models.Repositories
{
    public interface IUsuarioRepository:IRepository<Usuario>
    {
        Usuario GetUsuarioActivoWhere(Expression<Func<Usuario, bool>> predicate);

        IEnumerable<Usuario> GetUsuariosActivosWithRoles();
        IEnumerable<Usuario> GetUsuariosActivosWithRolesWhere(Expression<Func<Usuario, bool>> predicate);
        IEnumerable<Usuario> GetUsuariosNoActivosWithRolesWhere(Expression<Func<Usuario, bool>> predicate);
    }
}
