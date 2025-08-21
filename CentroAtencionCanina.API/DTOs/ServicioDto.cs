namespace CentroAtencionCanina.API.DTOs
{
    public class ServicioDto
    {
        public int ServicioId { get; set; }
        public string NombreServicio { get; set; }
        public string? Detalle { get; set; }
        public string? Medicamento { get; set; }
        public string Estado { get; set; }
    }

    public class CreateServicioDto
    {
        public string NombreServicio { get; set; }
        public string? Detalle { get; set; }
        public string? Medicamento { get; set; }
    }

    public class UpdateServicioDto : CreateServicioDto
    {
        public int ServicioId { get; set; }
        public string Estado { get; set; }
    }

    public class DeleteeServicioDto
    {
        public int ServicioId { get; set; }
        public string Estado { get; set; }
    }
    public class ServicioSelectDto
    {
        public int ServicioId { get; set; }
        public string NombreServicio { get; set; }
    }
}
