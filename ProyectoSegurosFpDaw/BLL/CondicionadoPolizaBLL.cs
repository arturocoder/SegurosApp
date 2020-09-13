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
    }
}