namespace CentroAtencionCanina.API.Models
{
    public class Raza
    {
        public int RazaId { get; set; }
        public string NombreRaza { get; set; }
        public int CategoriaId { get; set; } // 👈 clave foránea
        public Categoria Categoria { get; set; } // 👈 propiedad de navegación
        public string? Origen { get; set; }
        public string Tamano { get; set; }
        public int? EsperanzaVidaAnios { get; set; }
        public string? Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacion { get; set; }
    }
}
