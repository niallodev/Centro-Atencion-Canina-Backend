namespace CentroAtencionCanina.API.DTOs
{
    public class RazaDto
    {
        public int RazaId { get; set; }
        public string NombreRaza { get; set; }
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; }
        public string? Origen { get; set; }
        public string Tamano { get; set; }
        public int? EsperanzaVidaAnios { get; set; }
        public string? Descripcion { get; set; }
        public string Estado { get; set; }
    }

    public class CreateRazaDto
    {
        public string NombreRaza { get; set; }
        public int CategoriaId { get; set; }
        public string? Origen { get; set; }
        public string Tamano { get; set; }
        public int? EsperanzaVidaAnios { get; set; }
        public string? Descripcion { get; set; }
    }
    public class UpdateRazaDto : CreateRazaDto
    {
        public int RazaId { get; set; }
        public string Estado { get; set; }
    }

    public class DeleteRazaDto
    {
        public int RazaId { get; set; }
        public string Estado { get; set; }
    }
    public class RazaSelectDto
    {
        public int RazaId { get; set; }
        public string NombreRaza { get; set; }
        public int CategoriaId { get; set; }
    }
}
