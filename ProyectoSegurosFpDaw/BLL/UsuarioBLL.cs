﻿using Microsoft.Ajax.Utilities;
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
            if (IsThereOnlyOneUsuarioWithRolAdministrador() == true)
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
            int usuariosWithRolAdministrador = unitOfWork.Usuario.Where(c => c.rolId == 1 && c.activo == 1).Count();
            if (usuariosWithRolAdministrador == 1)
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
        public UsuarioSearchingFields GetSearchingField(string nombre, string apellido1, string dni, string email, string rolId)
        {
            UsuarioSearchingFields output = new UsuarioSearchingFields
            {
                SearchingParam = UsuarioSearchingParam.empty,
                SearchingRol = RolParam.allRoles
            };

            if (rolId.IsNullOrWhiteSpace() == false && rolId != "0")
            {
                output.SearchingRol = RolParam.rolId;
                output.SearchingValueRol = int.Parse(rolId);
            }


            if (nombre.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = UsuarioSearchingParam.nombre;
                output.SearchingValue.Add(nombre.Trim().ToUpperInvariant());

            }
            if (apellido1.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = UsuarioSearchingParam.apellido1;
                output.SearchingValue.Add(apellido1.Trim().ToUpperInvariant());

            }
            if (nombre.IsNullOrWhiteSpace() == false && apellido1.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = UsuarioSearchingParam.nombreAndApellido1;
            }
            if (dni.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = UsuarioSearchingParam.dni;
                output.SearchingValue.Add(dni.Trim().ToUpperInvariant());
            }
            if (email.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = UsuarioSearchingParam.email;
                output.SearchingValue.Add(email.Trim().ToUpperInvariant());
            }
            return output;
        }
        public List<Usuario> SearchUsuarios(UsuarioSearchingFields searchingFields)
        {
            var output = new List<Usuario>();

            // Rol (resto de campos vacíos).            
            if (searchingFields.SearchingParam == UsuarioSearchingParam.empty)
            {                               
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRoles().ToList();
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }

            // Nombre y 1er Apellido + Rol.               
            if (searchingFields.SearchingParam == UsuarioSearchingParam.nombreAndApellido1)
            {
                var nombre = searchingFields.SearchingValue[0];
                var apellido1 = searchingFields.SearchingValue[1];
                
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombre && c.apellido1Usuario == apellido1).ToList();
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombre && c.apellido1Usuario == apellido1 && c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }
            // Nombre + Rol.                
            if (searchingFields.SearchingParam == UsuarioSearchingParam.nombre)
            {
                var nombre = searchingFields.SearchingValue[0];
                
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombre).ToList();
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.nombreUsuario == nombre && c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }
            // Apellido + Rol.               
            if (searchingFields.SearchingParam == UsuarioSearchingParam.apellido1)
            {
                var apellido1 = searchingFields.SearchingValue[0];
                
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.apellido1Usuario == apellido1).ToList();
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.apellido1Usuario == apellido1 && c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }
            // NIF/NIE.                
            if (searchingFields.SearchingParam == UsuarioSearchingParam.dni)
            {
                var dni = searchingFields.SearchingValue[0];
               
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    var usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.dniUsuario == dni).ToList();

                    // Si no hay coincidencia en cliente activo, busca en clientes no activos                     
                    if (usuariosCoincidentes.Any() == false)
                    {
                        usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosNoActivosWithRolesWhere(c => c.dniUsuario == dni).ToList();
                    }
                    output = usuariosCoincidentes;
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.dniUsuario == dni && c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }
            // Email.
            if (searchingFields.SearchingParam == UsuarioSearchingParam.email)
            {
                var email = searchingFields.SearchingValue[0];
                
                if (searchingFields.SearchingRol == RolParam.allRoles)
                {
                    var usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.emailUsuario == email);
                    if (usuariosCoincidentes.Any() == false)
                    {
                        usuariosCoincidentes = unitOfWork.Usuario.GetUsuariosNoActivosWithRolesWhere(c => c.emailUsuario == email).ToList();
                    }
                    output = usuariosCoincidentes.ToList();
                }
                else
                {
                    output = unitOfWork.Usuario.GetUsuariosActivosWithRolesWhere(c => c.emailUsuario == email && c.rolId == searchingFields.SearchingValueRol).ToList();
                }
            }

            return output;
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