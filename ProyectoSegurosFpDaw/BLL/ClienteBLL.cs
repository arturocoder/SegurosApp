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
    public enum ClienteParam
    {
        id,
        dni,
        telefono,
        email,
        empty
    }
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

        public ClienteParam GetSearchingField(string clienteId, string dniCliente, string emailCliente, string telefonoCliente)
        {

            if (clienteId.IsNullOrWhiteSpace() == false) { return ClienteParam.id; }
            if (dniCliente.IsNullOrWhiteSpace() == false) { return ClienteParam.dni; }
            if (emailCliente.IsNullOrWhiteSpace() == false) { return ClienteParam.email; }
            if (telefonoCliente.IsNullOrWhiteSpace() == false) { return ClienteParam.telefono; }
            return ClienteParam.empty;

        }

        public List<Cliente> SearchClientes(ClienteParam parameter, string searchingValue)
        {
            var output = new List<Cliente>();
            searchingValue = searchingValue.Trim().ToUpperInvariant();

            if (parameter == ClienteParam.id)
            {
                bool success = Int32.TryParse(searchingValue, out int clienteId);
                if (success == false)
                {
                    return output;
                }
                output = unitOfWork.Cliente.Where(c => c.clienteId == clienteId).ToList();
            }
            if (parameter == ClienteParam.dni)
            {
                output = unitOfWork.Cliente.Where(c => c.dniCliente == searchingValue).ToList();
            }
            if (parameter == ClienteParam.email)
            {
                output = unitOfWork.Cliente.Where(c => c.emailCliente == searchingValue).ToList();
            }
            if (parameter == ClienteParam.telefono)
            {
                output = unitOfWork.Cliente.Where(c => c.telefonoCliente == searchingValue).ToList();
            }
            if (parameter == ClienteParam.empty)
            {
                output = unitOfWork.Cliente.Where(c => c.activo == 1).ToList();
            }

            return output;
        }










    }
}