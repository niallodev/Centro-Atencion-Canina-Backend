namespace CentroAtencionCanina.API.Models
{
    public class Cita
    {
        public int CitaId { get; set; }
        public int MascotaId { get; set; } // 👈 clave foránea
        public Mascota Mascota { get; set; } // 👈 propiedad de navegación
        public int ServicioId { get; set; } // 👈 clave foránea
        public Servicio Servicio { get; set; } // 👈 propiedad de navegación
        public DateTime FechaHora { get; set; }
        public int? ProfesionalId { get; set; } // 👈 clave foránea
        public Usuario Profesional { get; set; } // 👈 propiedad de navegación
        public string? Motivo { get; set; }
        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacion { get; set; }
    }
}
