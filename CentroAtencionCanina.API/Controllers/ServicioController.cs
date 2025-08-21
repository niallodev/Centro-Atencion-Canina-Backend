using CentroAtencionCanina.API.Data;
using CentroAtencionCanina.API.DTOs;
using CentroAtencionCanina.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentroAtencionCanina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ServicioController
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = _context.Servicios.AsQueryable();
            var servicios = await query
                .Select(s => new ServicioDto
                {
                    ServicioId = s.ServicioId,
                    NombreServicio = s.NombreServicio,
                    Detalle = s.Detalle,
                    Medicamento = s.Medicamento,
                    Estado = s.Estado,
                })
                .ToListAsync();
            return Ok(servicios);
        }

        // GET: ServicioController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Servicios.AsQueryable();
            query = query.Where(d => d.Estado != "Eliminado");

            var servicios = await query
                .Select(s => new ServicioSelectDto
                {
                    ServicioId = s.ServicioId,
                    NombreServicio = s.NombreServicio,
                })
                .ToListAsync();
            return Ok(servicios);
        }

        // GET: ServicioController/1
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var query = _context.Servicios.AsQueryable();
            var servicio = await query
                .Select(s => new ServicioDto
                {
                    ServicioId = s.ServicioId,
                    NombreServicio = s.NombreServicio,
                    Detalle = s.Detalle,
                    Medicamento = s.Medicamento,
                    Estado = s.Estado,
                })
                .FirstOrDefaultAsync(s => s.ServicioId == id);

            if (servicio == null) return NotFound();

            return Ok(servicio);
        }

        // POST: ServicioController/Create
        //[Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServicioDto nuevoServicioDto)
        {
            var nuevoServicio = new Servicio
            {
                NombreServicio = nuevoServicioDto.NombreServicio,
                Detalle = nuevoServicioDto.Detalle,
                Medicamento = nuevoServicioDto.Medicamento,
                Estado = "Activo",
                UsuarioCreacion = 1,
            };
            _context.Servicios.Add(nuevoServicio);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevoServicio.ServicioId }, null);
        }

        // PUT: ServicioController/Edit/5
        //[Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateServicioDto servicioActualizadaDto)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if(servicio == null || servicio.Estado == "Eliminado")
                    return NotFound();

                servicio.NombreServicio = servicioActualizadaDto.NombreServicio;
                servicio.Detalle = servicioActualizadaDto.Detalle;
                servicio.Medicamento = servicioActualizadaDto.Medicamento;
                servicio.Estado = servicioActualizadaDto.Estado;
                servicio.FechaModificacion = DateTime.Now;
                servicio.UsuarioModificacion = 1;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Servicios.Any(s => s.ServicioId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: ServicioController/Delete/5
        //[Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null || servicio.Estado == "Eliminado")
                return NotFound();

            servicio.Estado = "Eliminado";
            servicio.FechaEliminacion = DateTime.Now;
            servicio.UsuarioEliminacion = 1;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
