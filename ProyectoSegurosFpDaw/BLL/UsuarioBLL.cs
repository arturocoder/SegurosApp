using Microsoft.Ajax.Utilities;
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

        public UsuarioBLL(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public bool ValidateChangingRolAdministrador(Usuario usuario)
        {
            if (IsThereOnlyOneUsuarioWithRolAdministrador() ==true)
            {
                // Comprueba si el usuario a editar tiene un rol administrador .                      
                var usuarioEstadoPrevio = unitOfWork.Usuario.SingleOrDefaultNoTracking(c => c.usuarioId == usuario.usuarioId && c.rolId == 1);
                // Comprueba si está cambiando el rol a otro diferente de administrador.
                if (usuarioEstadoPrevio != null && usuarioEstadoPrevio.rolId != usuario.rolId)
                {
                    return false;
                }               
            }           
            return true;
        }
        public bool ValidateDeletingUsuarioRolAdministrador(Usuario usuario)
        {
            if (usuario.usuarioId == 1 && IsThereOnlyOneUsuarioWithRolAdministrador() == true)
            {
                return false;
            }            
            return true;
        }
        private bool IsThereOnlyOneUsuarioWithRolAdministrador()
        {
            int usuariosWithRolAdministrador = unitOfWork.Usuario.Find(c => c.rolId == 1 && c.activo == 1).Count();
            if(usuariosWithRolAdministrador == 1)
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
        public bool FieldsFormat(Usuario usuario)
        {
            if (IsValidFormat(usuario) == false)
            {
                return false;
            }
            usuario.nombreUsuario = usuario.nombreUsuario.Trim().ToUpperInvariant();
            usuario.apellido1Usuario = usuario.apellido1Usuario.Trim().ToUpperInvariant();
            usuario.apellido2Usuario = usuario.apellido2Usuario.Trim().ToUpperInvariant();
            usuario.dniUsuario = usuario.dniUsuario.Trim().ToUpperInvariant();
            usuario.emailUsuario = usuario.emailUsuario.Trim().ToUpperInvariant();
            usuario.password = usuario.password.Trim();
            return true;
        }

        private bool IsValidFormat(Usuario usuario)
        {
            if (usuario == null)
            {
                return false;
            }
            if (usuario.nombreUsuario.IsNullOrWhiteSpace() || usuario.apellido1Usuario.IsNullOrWhiteSpace()
                       || usuario.apellido2Usuario.IsNullOrWhiteSpace() || usuario.dniUsuario.IsNullOrWhiteSpace()
                       || usuario.emailUsuario.IsNullOrWhiteSpace() || usuario.password.IsNullOrWhiteSpace())
            {
                return false;
            }
            return true;

        }

        public void CreateNewUsuario(Usuario usuario)
        {
            usuario.fechaAlta = DateTime.Now;
            usuario.activo = 1;
            usuario.password = Encriptacion.GetSHA256(usuario.password);
            unitOfWork.Usuario.Add(usuario);
            unitOfWork.SaveChanges();
        }
        public void UpdateUsuario(Usuario usuario)
        {                         
            // Si se va a modificar rol a No operativo.   
            if (usuario.rolId == 2)
            {
                // Guarda la fecha de hoy como fecha de baja.
                DateTime hoy = DateTime.Now;
                usuario.fechaBaja = hoy;
            }
            else
            {
                usuario.fechaBaja = null;
            }            
            usuario.password = Encriptacion.GetSHA256(usuario.password);
            unitOfWork.Usuario.Update(usuario);
            unitOfWork.SaveChanges();
        }
        public void DeleteUsuario(Usuario usuario)
        {
            usuario.fechaBaja = DateTime.Now;
            // Asigna rol No Operativo.
            usuario.rolId = 2;
            usuario.activo = 0;
            
            unitOfWork.Usuario.Update(usuario);
            unitOfWork.SaveChanges();
        }
        
    }
}