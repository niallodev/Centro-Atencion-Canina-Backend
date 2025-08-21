namespace CentroAtencionCanina.API.DTOs
{
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public int IntentosFallidos { get; set; }
        public string FechaUltimoIntento { get; set; }
        public string Estado { get; set; }
    }

    public class CreateUsuarioDto
    {
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string Contrasenia { get; set; }
        public string Rol { get; set; }
    }

    public class UpdateUsuarioDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }
        public string Estado { get; set; }

    }

    public class DeleteUsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Estado { get; set; }
    }

    public class LoginDto
    {
        public string NombreUsuario { get; set; }
        public string Contrasenia { get; set; }
    }
    public class UsuarioSelectDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
    }

}
