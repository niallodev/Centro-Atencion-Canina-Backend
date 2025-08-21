namespace CentroAtencionCanina.API.DTOs
{
    public class MascotaDto
    {
        public int MascotaId { get; set; }
        
        public string NombreDueno { get; set; } 
        public string NombreMascota { get; set; }
        public string Especie { get; set; }
        public string NombreCategoria { get; set; }

        public string NombreRaza { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string? Color { get; set; }
        public decimal? Peso { get; set; }
        public string? InformacionAdicional { get; set; }
        public string Estado { get; set; }
    }

    public class CreateMascotaDto
    {
        public int DuenoId { get; set; }
        public string NombreMascota { get; set; }
        public string Especie { get; set; }
        public int RazaId { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string? Color { get; set; }
        public decimal? Peso { get; set; }
        public string? InformacionAdicional { get; set; }
    }

    public class UpdateMascotaDto : CreateMascotaDto
    {
        public int MascotaId { get; set; }
        public string Estado { get; set; }
    }
    public class DeleteMascotaDto
    {
        public int MascotaId { get; set; }
        public string Estado { get; set; }
    }
    public class UsuarioMascotaDto
    {
        public int MascotaId { get; set; }
        public int DuenoId { get; set; }
        public string NombreMascota { get; set; }
        public string Especie { get; set; }
        public string NombreRaza { get; set; }
        public string Estado { get; set; }
    }

    public class MascotaSelectDto
    {
        public int MascotaId { get; set; }
        public string NombreMascota { get; set; }
    }

}
