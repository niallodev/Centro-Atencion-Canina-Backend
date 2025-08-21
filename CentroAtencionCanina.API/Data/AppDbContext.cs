using CentroAtencionCanina.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CentroAtencionCanina.API.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Dueno> Duenos {get; set;}
        public DbSet<Mascota> Mascotas {get; set;}
        public DbSet<Cita> Citas {get; set;}
        public DbSet<Servicio> Servicios {get; set;}
        public DbSet<Raza> Razas {get; set;}
        public DbSet<Categoria> Categorias {get; set;}
    }
}
