using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public GestionPoliza GetLastGestionPoliza(int polizaId)
        {
            return unitOfWork.GestionPoliza
                .GetGestionesPolizaWithClienteCondicionadoTipoGestionWhere(c => c.polizaId == polizaId)
                .OrderByDescending(c => c.gestionPolizaId)
                .FirstOrDefault();

        }

        public bool FieldsFormat(GestionPoliza gestionPoliza, string clienteId)
        {
            if (IsValidFormat(gestionPoliza, clienteId) == false)
            {
                return false;
            }
            gestionPoliza.matricula = gestionPoliza.matricula.Trim().ToUpperInvariant();
            gestionPoliza.marcaVehiculo = gestionPoliza.marcaVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.modeloVehiculo = gestionPoliza.modeloVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.observaciones = gestionPoliza.observaciones.Trim();
            return true;
        }

        private bool IsValidFormat(GestionPoliza gestionPoliza, string clienteId)
        {
            if (gestionPoliza == null || clienteId.IsNullOrWhiteSpace())
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


        public bool FieldsFormatEdit(GestionPoliza gestionPoliza)
        {
            if (IsValidFormatEdit(gestionPoliza) == false)
            {
                return false;
            }
            gestionPoliza.matricula = gestionPoliza.matricula.Trim().ToUpperInvariant();
            gestionPoliza.marcaVehiculo = gestionPoliza.marcaVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.modeloVehiculo = gestionPoliza.modeloVehiculo.Trim().ToUpperInvariant();
            gestionPoliza.observaciones = gestionPoliza.observaciones.Trim();
            return true;
        }

        private bool IsValidFormatEdit(GestionPoliza gestionPoliza)
        {
            if (gestionPoliza == null)
            {
                return false;
            }
            if (
                gestionPoliza.matricula.IsNullOrWhiteSpace()
                || gestionPoliza.marcaVehiculo.IsNullOrWhiteSpace()
                || gestionPoliza.modeloVehiculo.IsNullOrWhiteSpace()
                || gestionPoliza.observaciones.IsNullOrWhiteSpace()
                )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validación de formato de la matrícula mediante expresión regular .
        ///<para>
        ///Obtenido de : https://www.laps4.com/comunidad/threads/necesito-funcion-javascript-para-validar-matriculas.186497/
        ///</para>
        /// </summary>
        /// <param name="matricula">matrícula</param>
        /// <returns>
        /// true => matricula correcta
        /// false => matricula incorrecta        
        /// </returns>
        public bool ValidarFormatoMatricula(string matricula)
        {

            // Matrícula nueva: 0123-ABC
            // Matrícula antigua: AB-0123-CS
            string pattern = @"(\d{4}-[\D\w]{3}|[\D\w]{1,2}-\d{4}-[\D\\w]{2})";
            Regex rgx = new Regex(pattern);
            if (rgx.IsMatch(matricula))
            {
                return true;
            }
            return false;
        }

        public bool ExistMatriculaInPolizasActivas(string matricula)
        {
            return unitOfWork.GestionPoliza.ExistMatriculaInPolizasActivas(matricula);
        }


        public void UpdateGestionPoliza(GestionPoliza gestionPoliza, Usuario usuarioLogado)
        {
            gestionPoliza.usuarioId = usuarioLogado.usuarioId;
            gestionPoliza.fechaGestion = DateTime.Now;
            // 3 => MODIFICACIÓN 
            gestionPoliza.tipoGestionId = 3;
            unitOfWork.GestionPoliza.Add(gestionPoliza);

            unitOfWork.SaveChanges();
        }

        public void DeleteGestionPoliza(GestionPoliza gestionPoliza, Usuario usuarioLogado, string motivoCancelacion, Poliza poliza)
        {
            gestionPoliza.usuarioId = usuarioLogado.usuarioId;
            gestionPoliza.fechaGestion = DateTime.Now;

            // Si es una póliza con fecha de inicio futura ,asigna mismo valor a fecha Inicio /Fin.
            if (gestionPoliza.fechaInicio > DateTime.Today)
            {
                gestionPoliza.fechaFin = gestionPoliza.fechaInicio;
            }
            else
            {
                gestionPoliza.fechaFin = DateTime.Today;

            }

            // Tipo de gestión:
            // 2 => BAJA 
            gestionPoliza.tipoGestionId = 2;
            gestionPoliza.observaciones = "Póliza cancelada por usuario : " + usuarioLogado.emailUsuario + ". \nMotivo : " + motivoCancelacion.Trim();

            // Crea nueva gestión póliza
            unitOfWork.GestionPoliza.Add(gestionPoliza);
            poliza.fechaDesactivado = DateTime.Now;
            poliza.activo = 0;

            unitOfWork.SaveChanges();
            throw new Exception();
        }
        public void UnDeleteGestionPoliza(int polizaId)
        {
            // Si no se ha podido dar de baja la póliza
            // Comprueba que se haya cambiado el estado activo           
            var polizaModificada = unitOfWork.Poliza.Get(polizaId);
            if (polizaModificada.activo == 0)
            {
                polizaModificada.activo = 1;              

            }
            // Comprueba que se haya creado una gestión Póliza con estado baja y la elimina
            var gestionPolizaModificada = unitOfWork.GestionPoliza
                .Where(c => c.polizaId == polizaId && c.tipoGestionId == 2).FirstOrDefault();
            if (gestionPolizaModificada != null)
            {
                unitOfWork.GestionPoliza.Remove(gestionPolizaModificada);
            }
            
            unitOfWork.SaveChanges();
        }
    }
}