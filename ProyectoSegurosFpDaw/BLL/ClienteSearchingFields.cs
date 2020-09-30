using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public enum ClienteSearchingParam
    {
        id,
        dni,
        telefono,
        email,
        empty
    }
    public class ClienteSearchingFields
    {
        public ClienteSearchingParam SearchingParam { get; set; }        
        public string Value { get; set; }
        public int ValueId { get; set; }

    }
}