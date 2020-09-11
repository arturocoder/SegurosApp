using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public class AccesoBLL
    {
       
        private UnitOfWork unitOfWork;

        public AccesoBLL(ProyectoSegurosDbEntities context)
        {
            unitOfWork = new UnitOfWork(context);
        }        
      

        public Usuario GetAuthenticatedUsuario(string email, string password)
        {
            var passwordEncrypted = Encriptacion.GetSHA256(password);
            return unitOfWork.Usuario
                       .SingleOrDefault(c => c.emailUsuario == email.Trim() && c.password == passwordEncrypted);
        }
    }
}