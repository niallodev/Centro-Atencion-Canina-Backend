namespace CentroAtencionCanina.API.DTOs
{
    public class CategoriaDto
    {
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; }
        public string? Descripcion { get; set; }
        public string Estado { get; set; }
    }

    public class CreateCategoriaDto
    {
        public string NombreCategoria { get; set; }
        public string? Descripcion { get; set; }

    }

    public class UpdateCategoriaDto : CreateCategoriaDto
    {
        public int CategoriaId { get; set; }
        public string Estado { get; set; }

    }
    public class DeleteCategoriaDto
    {
        public int CategoriaId { get; set; }
        public string Estado { get; set; }

    }
    public class CategoriaSelectDto
    {
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; }
    }
}
