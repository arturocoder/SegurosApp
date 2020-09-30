using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ProyectoSegurosFpDaw.Controllers.ClientesController;

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
        public void DeleteCliente(Cliente cliente)
        {
            cliente.fechaDesactivado = DateTime.Now;
            cliente.activo = 0;
            unitOfWork.SaveChanges();
        }

       
        public ClienteSearchingFields GetSearchingField(string clienteId, string dniCliente, string email, string telefonoCliente)
        {
            ClienteSearchingFields output = new ClienteSearchingFields()
            {
                SearchingParam= ClienteSearchingParam.empty,                
            };
            if (clienteId.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = ClienteSearchingParam.id;
                output.ValueId = int.Parse(clienteId);
                
            }
            if (dniCliente.IsNullOrWhiteSpace() == false) 
            {
                output.SearchingParam = ClienteSearchingParam.dni;
                output.Value = dniCliente.Trim().ToUpperInvariant();
            }
            if (email.IsNullOrWhiteSpace() == false) 
            {
                output.SearchingParam = ClienteSearchingParam.email;
                output.Value = email.Trim().ToUpperInvariant();
            }
            if (telefonoCliente.IsNullOrWhiteSpace() == false)
            {
                output.SearchingParam = ClienteSearchingParam.telefono;
                output.Value = telefonoCliente.Trim().ToUpperInvariant();
            }
            return output;

        }       


        public List<Cliente> SearchClientes(ClienteSearchingFields searchingFields)
        {
            var output = new List<Cliente>();            

            if (searchingFields.SearchingParam == ClienteSearchingParam.id)
            {                
                output = unitOfWork.Cliente.Where(c => c.clienteId == searchingFields.ValueId).ToList();
            }
            if (searchingFields.SearchingParam == ClienteSearchingParam.dni)
            {
                output = unitOfWork.Cliente.Where(c => c.dniCliente == searchingFields.Value).ToList();
            }
            if (searchingFields.SearchingParam == ClienteSearchingParam.email)
            {
                output = unitOfWork.Cliente.Where(c => c.emailCliente == searchingFields.Value).ToList();
            }
            if (searchingFields.SearchingParam == ClienteSearchingParam.telefono)
            {
                output = unitOfWork.Cliente.Where(c => c.telefonoCliente == searchingFields.Value).ToList();
            }
            if (searchingFields.SearchingParam == ClienteSearchingParam.empty)
            {
                output = unitOfWork.Cliente.Where(c => c.activo == 1).ToList();
            }

            return output;
        }










    }
}