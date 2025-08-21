namespace CentroAtencionCanina.API.Models
{
    public class Mascota
    {
        public int MascotaId { get; set; }
        public int DuenoId { get; set; } // 👈 clave foránea
        public Dueno Dueno { get; set; } // 👈 propiedad de navegación
        public string NombreMascota { get; set; }
        public string Especie { get; set; }
        public int RazaId { get; set; } // 👈 clave foránea
        public Raza Raza { get; set; } // 👈 propiedad de navegación
        public DateTime? FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string? Color { get; set; }
        public decimal? Peso { get; set; }
        public string? InformacionAdicional { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacion { get; set; }
    }
}
