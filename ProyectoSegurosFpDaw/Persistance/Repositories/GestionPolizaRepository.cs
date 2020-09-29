using ProyectoSegurosFpDaw.Models;
using ProyectoSegurosFpDaw.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ProyectoSegurosFpDaw.Persistance.Repositories
{

    public class GestionPolizaRepository : Repository<GestionPoliza>, IGestionPolizaRepository
    {
        public GestionPolizaRepository(ProyectoSegurosDbEntities context) : base(context)
        {
        }

        public ProyectoSegurosDbEntities ProyectoSegurosContext
        {
            get { return Context as ProyectoSegurosDbEntities; }
        }


        public GestionPoliza GetGestionPolizaWithClienteCondicionadoTipoGestion(int gestionPolizaId)
        {
            return ProyectoSegurosContext.GestionPoliza
                .Include(c => c.Poliza.Cliente)
                .Include(c => c.CondicionadoPoliza)
                .Include(c => c.TipoGestion)
                .Where(c => c.gestionPolizaId == gestionPolizaId).FirstOrDefault();
        }
        public IEnumerable<GestionPoliza> GetGestionesPolizaWithClienteCondicionadoTipoGestionWhere(Expression<Func<GestionPoliza, bool>> predicate)
        {
            return ProyectoSegurosContext.GestionPoliza
                .Include(c => c.Poliza.Cliente)
                .Include(c => c.CondicionadoPoliza)
                .Include(c => c.TipoGestion)
                .Where(predicate)
                .ToList();

        }


        public bool ExistMatriculaInPolizasActivas(string matricula)
        {
            var polizasActivas = ProyectoSegurosContext.Poliza.Where(c => c.activo == 1);
            // Get Last GestionPolizaId on every Poliza Activa
            var gestionesPolizaLast = from gestiones in ProyectoSegurosContext.GestionPoliza
                                      join polizas in polizasActivas on gestiones.polizaId equals polizas.polizaId
                                      group gestiones by gestiones.polizaId
                                      into g
                                      select g.Max(c => c.gestionPolizaId);

            var gestionPolizaIdCoincidenteConMatricula = from gestiones in ProyectoSegurosContext.GestionPoliza
                                                         join gest in gestionesPolizaLast on gestiones.gestionPolizaId equals gest
                                                         where gestiones.matricula == matricula
                                                         select gestiones.gestionPolizaId;
            if (gestionPolizaIdCoincidenteConMatricula.Any())
            {
                return true;
            }
            return false;

        }

        public IEnumerable<GestionPoliza> GetLastGestionPolizaWithPolizaByDate(DateTime fechaInicio, DateTime fechaFinal)
        {
            var output = new List<GestionPoliza>();

            // Obtiene el id de las pólizas que coinciden con el rango de fecha de Alta.
            var polizasCoincidentes =
                 from gestiones in ProyectoSegurosContext.GestionPoliza
                 join polizas in ProyectoSegurosContext.Poliza on gestiones.polizaId equals polizas.polizaId
                 where gestiones.fechaInicio < fechaFinal && gestiones.fechaInicio > fechaInicio
                 select new { Poliza = polizas.polizaId };

            // Si hay resultados coincidentes
            if (polizasCoincidentes.Any())
            {
                // Recorre la query (obviando las pólizas repetidas (distinct)) 
                foreach (var item in polizasCoincidentes.Distinct())
                {
                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º)
                    var ultimaGestion = ProyectoSegurosContext.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                        .Where(c => c.polizaId == item.Poliza)
                        .OrderByDescending(c => c.gestionPolizaId)
                        .FirstOrDefault();

                    // Añade a la lista 
                    output.Add(ultimaGestion);
                }

            }
            return output;
        }

        public IEnumerable<GestionPoliza> GetLastGestionPolizaWithPolizaByDate(DateTime fechaInicio, DateTime fechaFinal, int estadoPoliza)
        {
            var output = new List<GestionPoliza>();

            // Obtiene el id de las pólizas que coinciden con 
            // el rango de fecha de Alta + estado Póliza (activo/No activo).
            var polizasCoincidentes =
                 from gestiones in ProyectoSegurosContext.GestionPoliza
                 join polizas in ProyectoSegurosContext.Poliza on gestiones.polizaId equals polizas.polizaId
                 where gestiones.fechaInicio < fechaFinal && gestiones.fechaInicio > fechaInicio
                     && polizas.activo == estadoPoliza
                 select new { Poliza = polizas.polizaId };

            // Si hay resultados coincidentes
            if (polizasCoincidentes.Any())
            {
                // Recorre la query (obviando las pólizas repetidas (distinct)) 
                foreach (var item in polizasCoincidentes.Distinct())
                {
                    // Selecciona la última gestión de cada póliza (orden descendente => selecciona la 1º)
                    var ultimaGestion = ProyectoSegurosContext.GestionPoliza.Include(c => c.Poliza).Include(c => c.Poliza.Cliente)
                        .Where(c => c.polizaId == item.Poliza)
                        .OrderByDescending(c => c.gestionPolizaId)
                        .FirstOrDefault();

                    // Añade a la lista 
                    output.Add(ultimaGestion);
                }

            }
            return output;
        }
    }
}