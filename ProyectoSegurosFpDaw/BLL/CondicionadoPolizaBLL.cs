using Microsoft.Ajax.Utilities;
using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public class CondicionadoPolizaBLL
    {
        private UnitOfWork unitOfWork;
       
        public CondicionadoPolizaBLL(UnitOfWork unitOfWork)
        {
            this.unitOfWork =  unitOfWork;
        }
        public bool AnyCondicionadoWithTipoCondicionado(string tipoCondicionado)
        {
            return unitOfWork.CondicionadoPoliza.Any(c => c.tipoCondicionado == tipoCondicionado);
        }
        private bool IsValidFormat(CondicionadoPoliza condicionadoPoliza)
        {
            if(condicionadoPoliza == null)
            {
                return false;
            }
            if (condicionadoPoliza.tipoCondicionado.IsNullOrWhiteSpace() || condicionadoPoliza.garantias.IsNullOrWhiteSpace())
            {
                    return false;
            }            
            return true;
        }
        public bool FieldsFormat(CondicionadoPoliza condicionadoPoliza)
        {
            if (IsValidFormat(condicionadoPoliza) == false)
            {
                return false;
            }
            condicionadoPoliza.tipoCondicionado = condicionadoPoliza.tipoCondicionado.Trim().ToUpperInvariant();
            condicionadoPoliza.garantias = condicionadoPoliza.garantias.Trim();
            return true;
        }

        public void CreateNewCondicionadoPoliza(CondicionadoPoliza condicionadoPoliza)
        {
            condicionadoPoliza.activo = 1;
            unitOfWork.CondicionadoPoliza.Add(condicionadoPoliza);
            unitOfWork.SaveChanges();
        }

        public void DeleteCondicionado(CondicionadoPoliza condicionadoPoliza)
        {
            condicionadoPoliza.fechaDesactivado = DateTime.Now;
            condicionadoPoliza.activo = 0;
            unitOfWork.CondicionadoPoliza.Update(condicionadoPoliza);
            unitOfWork.SaveChanges();
        }
        public void UnDeleteCondicionado(CondicionadoPoliza condicionadoPoliza)
        {
            condicionadoPoliza.fechaDesactivado = null;
            condicionadoPoliza.activo = 1;
            unitOfWork.CondicionadoPoliza.Update(condicionadoPoliza);
            unitOfWork.SaveChanges();
        }
        public bool AnyPolizaActivaWithThisCondicionado(CondicionadoPoliza condicionadoPoliza)
        {
            return unitOfWork.GestionPoliza.ExistCondicionadoInPolizasActivas(condicionadoPoliza.condicionadoPolizaId);
        }
        public List<int> GetPolizasActivasIdWithThisCondicionado(CondicionadoPoliza condicionadoPoliza)
        {            
            List<int> output = unitOfWork.GestionPoliza.GetLastGestionesPolizaActivasByCondicionadoPoliza(condicionadoPoliza).Select(c => c.polizaId).ToList();
            return output;            
        }
    }
}