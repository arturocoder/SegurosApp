using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public enum PolizaSearchingParam
    {
        polizaId,
        matricula,
        dniCliente,
        telefonoCliente, 
        empty
    }
    public enum EstadoPolizaParam
    {
        noActivo,
        activo,
        todos
    }
    public class PolizaSearchingFields
    {
        public int PolizaId { get; set; }
        public string Value { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public PolizaSearchingParam SearchingParam { get; set; }
        public EstadoPolizaParam EstadoPoliza { get; set; }
    }
}