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
        private bool IsValid(CondicionadoPoliza condicionadoPoliza)
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
        public bool FormatFields(CondicionadoPoliza condicionadoPoliza)
        {
            if (IsValid(condicionadoPoliza) == false)
            {
                return false;
            }
            condicionadoPoliza.tipoCondicionado = condicionadoPoliza.tipoCondicionado.Trim().ToUpperInvariant();
            condicionadoPoliza.garantias = condicionadoPoliza.garantias.Trim();
            return true;
        }
    }
}