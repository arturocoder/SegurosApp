﻿using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public class ClienteBLL
    {
        private UnitOfWork unitOfWork;

        public ClienteBLL(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public bool FieldsFormat(Cliente cliente)
        {
            if (IsValidFormat(cliente) == false)
            {
                return false;
            }
            cliente.nombreCliente = cliente.nombreCliente.Trim().ToUpperInvariant();
            cliente.apellido1Cliente = cliente.apellido1Cliente.Trim().ToUpperInvariant();
            cliente.apellido2Cliente = cliente.apellido2Cliente.Trim().ToUpperInvariant();
            cliente.dniCliente = cliente.dniCliente.Trim().ToUpperInvariant();
            cliente.emailCliente = cliente.emailCliente.Trim().ToUpperInvariant();
            cliente.telefonoCliente = cliente.telefonoCliente.Trim();
            return true;
        }

        private bool IsValidFormat(Cliente cliente)
        {
            if (cliente == null)
            {
                return false;
            }
            if (cliente.nombreCliente.IsNullOrWhiteSpace() || cliente.apellido1Cliente.IsNullOrWhiteSpace()
                 || cliente.apellido2Cliente.IsNullOrWhiteSpace() || cliente.dniCliente.IsNullOrWhiteSpace()
                 || cliente.emailCliente.IsNullOrWhiteSpace() || cliente.telefonoCliente.IsNullOrWhiteSpace())
            {
                return false;
            }
            return true;

        }

        public bool AnyClienteWithDni(string dni)
        {
            return unitOfWork.Cliente.Any(c => c.dniCliente == dni);
        }

        public void CreateNewCliente(Cliente cliente)
        {
            cliente.activo = 1;
            unitOfWork.Cliente.Add(cliente);
            unitOfWork.SaveChanges();
        }
        public void UpdateCliente()
        {            
            unitOfWork.SaveChanges();
        }
    }
}