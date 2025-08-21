namespace CentroAtencionCanina.API.DTOs
{
    public class CitaDto
    {
        public int CitaId { get; set; }
        public int MascotaId { get; set; } 
        public string NombreMascota { get; set; }
        public int ServicioId { get; set; } 
        public string NombreServicio { get; set; } 
        public DateTime FechaHora { get; set; }
        public int ProfesionalId { get; set; } 
        public string NombreProfesional { get; set; }
        public string? Motivo { get; set; }
        public string Estado { get; set; }
    }

    public class CreateCitaDto
    {
        public int MascotaId { get; set; }
        public int ServicioId { get; set; }
        public DateTime FechaHora { get; set; }
        public int? ProfesionalId { get; set; }
        public string? Motivo { get; set; }
    }

    public class UpdateCitaDto : CreateCitaDto
    {
        public int CitaId { get; set; }
        public string Estado { get; set; }
    }

    public class DeleteCitaDto
    {
        public int CitaId { get; set; }
        public string Estado { get; set; }
    }
}
