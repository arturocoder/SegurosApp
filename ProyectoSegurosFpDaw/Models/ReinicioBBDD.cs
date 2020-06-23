using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoSegurosFpDaw.Models
{
    public static class ReinicioBBDD
    {
        /// <summary>
        /// LLama a un procedimiento(procedure store)
        /// de la BBDD, eliminando todos los registros de la 
        /// BBDD, y cargando los registros originales
        /// </summary>
        /// <returns></returns>
        public static bool ReiniciarBBDD()
        {
            using (ProyectoSegurosDbEntities db = new ProyectoSegurosDbEntities())
            {
                try
                {
                    // Ejecuta el procedure store de la base de datos
                    db.Database.ExecuteSqlCommand("EXEC dbo.ReiniciarData");
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            
        }
    }
}