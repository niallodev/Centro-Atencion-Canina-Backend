using System.ComponentModel.DataAnnotations;

namespace CentroAtencionCanina.API.Models
{
    public class Servicio
    {
        [Key]
        public int ServicioId { get; set; }
        public string NombreServicio { get; set; }
        public string? Detalle { get; set; }
        public string? Medicamento { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public int? UsuarioEliminacion { get; set; }

    }
}
