namespace CentroAtencionCanina.API.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public string Rol { get; set; }
        public string? Token { get; set; }
        public string Estado { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? FechaUltimoIntento { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacion { get; set; }
    }
}
