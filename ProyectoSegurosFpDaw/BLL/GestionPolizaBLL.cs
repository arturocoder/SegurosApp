using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public class GestionPolizaBLL
    {

        private UnitOfWork unitOfWork;

        public GestionPolizaBLL(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// GET: lista con histórico de todas las gestiones pólizas de una póliza
        /// ordenadas por fecha
        /// </summary>
        /// <param name="id">póliza Id</param>
        /// <returns>lista de gestionPolizas</returns>
        public List<GestionPoliza> GetHistoricoPoliza(int polizaId)
        {
            return unitOfWork.GestionPoliza.GetGestionesPolizaWithClienteCondicionadoTipoGestionWhere(c => c.polizaId == polizaId)
                .OrderBy(c => c.fechaGestion)
                .ToList();
        }

        public bool FieldsFormat(GestionPoliza gestionPoliza,string clienteId)
        {
            if (IsValidFormat(gestionPoliza,clienteId) == false)
            {
                return false;
            }
            gestionPoliza.matricula = gestionPoliza.matricula.Trim().ToUpperInvariant();
            gestionPoliza.marcaVehiculo = gestionPoliza.marcaVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.modeloVehiculo = gestionPoliza.modeloVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.observaciones = gestionPoliza.observaciones.Trim();
            return true;
        }

        private bool IsValidFormat(GestionPoliza gestionPoliza,string clienteId)
        {
            if (gestionPoliza == null||clienteId.IsNullOrWhiteSpace())
            {
                return false;
            }
            if (gestionPoliza.matricula.IsNullOrWhiteSpace() || gestionPoliza.marcaVehiculo.IsNullOrWhiteSpace()
                        || gestionPoliza.modeloVehiculo.IsNullOrWhiteSpace() || gestionPoliza.observaciones.IsNullOrWhiteSpace()
                        || gestionPoliza.fechaInicio == null || gestionPoliza.fechaFin == null)
            {
                return false;
            }
            return true;

        }
    }
}