using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.BLL
{
    public enum UsuarioSearchingParam
    {
        nombre,
        apellido1,
        nombreAndApellido1,
        dni,
        email,
        empty
    }
    public enum UsuarioSearchingRolParam
    {
        allRoles,
        rolId
    }
    public class UsuarioSearching
    {
        public UsuarioSearchingParam SearchingParam { get; set; }
        public UsuarioSearchingRolParam SearchingRol { get; set; }
        public List<string> SearchingValue { get; set; } = new List<string>();
        public int SearchingValueRol { get; set; }
    }
}