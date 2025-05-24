using System;

namespace SegurosApp.Api.Models
{
    public class GestionPoliza
    {
        public int GestionPolizaId { get; set; }
        public string Matricula { get; set; }
        public string MarcaVehiculo { get; set; }
        public string ModeloVehiculo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Precio { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaGestion { get; set; }
        public int CondicionadoPolizaId { get; set; }
        public int TipoGestionId { get; set; }
        public int PolizaId { get; set; }
        public int UsuarioId { get; set; }

        public CondicionadoPoliza CondicionadoPoliza { get; set; }
        public Poliza Poliza { get; set; }
        public TipoGestion TipoGestion { get; set; }
        public Usuario Usuario { get; set; }
    }
}
