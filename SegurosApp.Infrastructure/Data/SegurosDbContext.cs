using Microsoft.EntityFrameworkCore;
using SegurosApp.Domain.Entities;

namespace SegurosApp.Infrastructure.Data
{
    public class SegurosDbContext : DbContext
    {
        public SegurosDbContext(DbContextOptions<SegurosDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Poliza> Polizas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<CondicionadoPoliza> CondicionadoPolizas { get; set; }
        public DbSet<GestionPoliza> GestionPolizas { get; set; }
        public DbSet<TipoGestion> TipoGestiones { get; set; }
        public DbSet<RolPermiso> RolPermisos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Modulo> Modulos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
