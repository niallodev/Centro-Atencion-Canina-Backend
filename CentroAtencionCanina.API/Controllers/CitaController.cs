using CentroAtencionCanina.API.Data;
using CentroAtencionCanina.API.DTOs;
using CentroAtencionCanina.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CentroAtencionCanina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CitaController
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = HttpContext.User;
            var query = _context.Citas.AsQueryable();
            query = query.Where(c => c.Estado != "Eliminada" && c.Estado != "Cancelada");
            var citas = await query
                .Include(c => c.Mascota)
                .Include(c => c.Servicio)
                .Include(c => c.Profesional)
                .Select(c => new CitaDto
                {
                    CitaId = c.CitaId,
                    NombreMascota = c.Mascota.NombreMascota,
                    NombreServicio = c.Servicio.NombreServicio,
                    FechaHora = c.FechaHora,
                    NombreProfesional = c.Profesional.NombreCompleto,
                    Motivo = c.Motivo,
                    Estado = c.Estado,
                })
                .ToListAsync();
            return Ok(citas);
        }

        // GET: CitaController
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("cita/{id}")]
        public async Task<IActionResult> GetServiceCitaAll(int id)
        {
            var user = HttpContext.User;
            var query = _context.Citas.AsQueryable();
            query = query.Where(c => c.Estado != "Eliminada" && c.Estado != "Cancelada");
          
            var citas = await query
                .Include(c => c.Mascota)
                .Include(c => c.Servicio)
                .Include(c => c.Profesional)
                .Where(c => id == 1 ? c.ServicioId == 1 || c.ServicioId == 5 : c.ServicioId == id)
                .Select(c => new CitaDto
                {
                    CitaId = c.CitaId,
                    NombreMascota = c.Mascota.NombreMascota,
                    NombreServicio = c.Servicio.NombreServicio,
                    FechaHora = c.FechaHora,
                    NombreProfesional = c.Profesional.NombreCompleto,
                    Motivo = c.Motivo,
                    Estado = c.Estado,
                })
                .ToListAsync();
            return Ok(citas);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var query = _context.Citas.AsQueryable();

            var pendientes = await query.CountAsync(c => c.Estado == "Pendiente" || c.Estado == "Confirmada");
            var realizadas = await query.CountAsync(c => c.Estado == "Realizada");
            var canceladas = await query.CountAsync(c => c.Estado == "Cancelada" || c.Estado == "Eliminada");

            return Ok(new {
                Pendientes = pendientes,
                Realizadas = realizadas,
                Canceladas = canceladas
            });
        }

        // GET: CitaController/1
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var query = _context.Citas.AsQueryable();
            var cita = await query
                .Include(m => m.Mascota)
                .Include(m => m.Servicio)
                .Include(m => m.Profesional)
                .Select(c => new CitaDto
                {
                    CitaId = c.CitaId,
                    NombreMascota = c.Mascota.NombreMascota,
                    NombreServicio = c.Servicio.NombreServicio,
                    FechaHora = c.FechaHora,
                    NombreProfesional = c.Profesional.NombreCompleto,
                    Motivo = c.Motivo,
                    Estado = c.Estado,
                })
                .FirstOrDefaultAsync(c => c.CitaId == id);

            if (cita == null) return NotFound();

            return Ok(cita);
        }

        // POST: CitaController/Create
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCitaDto nuevaCitaDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var nuevaCita = new Cita()
            {
                MascotaId = nuevaCitaDto.MascotaId,
                ServicioId = nuevaCitaDto.ServicioId,
                FechaHora = nuevaCitaDto.FechaHora,
                ProfesionalId = nuevaCitaDto.ProfesionalId,
                Motivo = nuevaCitaDto.Motivo,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = userId,
            };
            _context.Citas.Add(nuevaCita);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevaCita.CitaId }, null);
        }


        // PUT: CitaController/Edit/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCitaDto citaActualizadaDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var cita = await _context.Citas.FindAsync(id);
                if (cita == null || cita.Estado == "Eliminada" || cita.Estado == "Cancelada")
                    return NotFound();

                cita.MascotaId = citaActualizadaDto.MascotaId;
                cita.ServicioId = citaActualizadaDto.ServicioId;
                cita.FechaHora = citaActualizadaDto.FechaHora;
                if (citaActualizadaDto.ProfesionalId.HasValue)
                    cita.ProfesionalId = citaActualizadaDto.ProfesionalId;
                cita.Motivo = citaActualizadaDto.Motivo;
                cita.Estado = citaActualizadaDto.Estado;
                cita.FechaModificacion = DateTime.Now;
                cita.UsuarioModificacion = userId;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Citas.Any(c => c.CitaId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: CitaController/Delete/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null || cita.Estado == "Eliminada" || cita.Estado == "Cancelada")
                return NotFound();

            cita.Estado = "Eliminada";
            cita.FechaEliminacion = DateTime.Now;
            cita.UsuarioEliminacion = userId;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

