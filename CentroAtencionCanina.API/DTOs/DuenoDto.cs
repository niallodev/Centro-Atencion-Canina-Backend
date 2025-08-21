namespace CentroAtencionCanina.API.DTOs
{
    public class DuenoDto
    {
        public int DuenoId { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string? Email { get; set; }
        public string Estado { get; set; }
    }

    public class CreateDuenoDto
    {
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateDuenoDto : CreateDuenoDto
    {
        public int DuenoId { get; set; }
        public string Estado { get; set; }
    }

    public class DeleteDuenoDto
    {
        public int DuenoId { get; set; }
        public string Estado { get; set; }
    }
    public class DuenoSelectDto
    {
        public int DuenoId { get; set; }
        public string NombreCompleto { get; set; }
    }
}
