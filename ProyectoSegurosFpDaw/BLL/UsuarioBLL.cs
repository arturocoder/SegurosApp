using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public class UsuarioBLL 
    {
        
        private UnitOfWork unitOfWork;

        public UsuarioBLL(ProyectoSegurosDbEntities context)
        {            
            unitOfWork = new UnitOfWork(context);
        }

        public bool IsThereJustOneUsuarioActivoWithRolAdministrador()
        {

            var numeroAdmones = unitOfWork.Usuario.Find(c => c.rolId == 1 && c.activo == 1).Count();
            if (numeroAdmones == 1)
            {
                return true;
            }
            return false;

        } 
        public bool AnyUsuarioWithEmail(string email)
        {
            return unitOfWork.Usuario.Any(c => c.emailUsuario == email);
        }
        public bool AnyUsuarioWithDni(string dni)
        {
            return unitOfWork.Usuario.Any(c => c.dniUsuario == dni);
        }

        public Usuario GetAuthenticatedUsuario(string email, string password)
        {
            return unitOfWork.Usuario
                       .SingleOrDefault(c => c.emailUsuario == email.Trim() && c.password == password);
                       
        }
    }
}