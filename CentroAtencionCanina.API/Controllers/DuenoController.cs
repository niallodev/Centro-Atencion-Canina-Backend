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
    public class DuenoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DuenoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DuenoController
        //[Authorize(Roles = "")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = HttpContext.User;
            var esAdmin = user.IsInRole("Administrador");
            var query = _context.Duenos.AsQueryable();

            if (!esAdmin)
            {
                // El recepcionista NO ve dueños eliminadas
                query = query.Where(d => d.Estado != "Eliminado");
            }

            var duenos = await query
                .Select(d => new DuenoDto
                {
                    DuenoId = d.DuenoId,
                    NombreCompleto = d.NombreCompleto,
                    TipoIdentificacion = d.TipoIdentificacion,
                    NumeroIdentificacion = d.NumeroIdentificacion,
                    Direccion = d.Direccion,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    Estado = d.Estado,
                })
                .ToListAsync();
            return Ok(duenos);
        }

        // GET: DuenoController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Duenos.AsQueryable();
            // El recepcionista NO ve dueños eliminadas
            query = query.Where(d => d.Estado != "Eliminado");
           
            var duenos = await query
                .Select(d => new DuenoSelectDto
                {
                    DuenoId = d.DuenoId,
                    NombreCompleto = d.NombreCompleto,
                })
                .ToListAsync();
            return Ok(duenos);
        }

        // GET: DuenoController/1
        //[Authorize(Roles = "")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var user = HttpContext.User;
            var esAdmin = user.IsInRole("Administrador");
            var query = _context.Duenos.AsQueryable();

            if (!esAdmin)
            {
                // El recepcionista NO ve dueños eliminadas
                query = query.Where(d => d.Estado != "Eliminado");
            }
            var dueno = await query
                .Select(d => new DuenoDto
                {
                    DuenoId = d.DuenoId,
                    NombreCompleto = d.NombreCompleto,
                    TipoIdentificacion = d.TipoIdentificacion,
                    NumeroIdentificacion = d.NumeroIdentificacion,
                    Direccion = d.Direccion,
                    Telefono = d.Telefono,
                    Email = d.Email,
                    Estado = d.Estado,
                })
                .FirstOrDefaultAsync(d => d.DuenoId == id);

            if (dueno == null) return NotFound();

            return Ok(dueno);
        }

        // POST: DuenoController/Create
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDuenoDto nuevoDuenoDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var nuevoDueno = new Dueno
            {
                NombreCompleto = nuevoDuenoDto.NombreCompleto,
                TipoIdentificacion = nuevoDuenoDto.TipoIdentificacion,
                NumeroIdentificacion = nuevoDuenoDto.NumeroIdentificacion,
                Direccion = nuevoDuenoDto.Direccion,
                Telefono = nuevoDuenoDto.Telefono,
                Email = nuevoDuenoDto.Email,
                Estado = "Activo",
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = userId
            };
            _context.Duenos.Add(nuevoDueno);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevoDueno.DuenoId }, null);
        }


        // PUT: DuenoController/Edit/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDuenoDto duenoActualizadaDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var dueno = await _context.Duenos.FindAsync(id);
                if (dueno == null || dueno.Estado == "Eliminado")
                    return NotFound();

                dueno.NombreCompleto = duenoActualizadaDto.NombreCompleto;
                dueno.TipoIdentificacion = duenoActualizadaDto.TipoIdentificacion;
                dueno.NumeroIdentificacion = duenoActualizadaDto.NumeroIdentificacion;
                dueno.Direccion = duenoActualizadaDto.Direccion;
                dueno.Telefono = duenoActualizadaDto.Telefono;
                dueno.Email = duenoActualizadaDto.Email;
                dueno.Estado = duenoActualizadaDto.Estado;
                dueno.FechaModificacion = DateTime.Now;
                dueno.UsuarioModificacion = userId;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Duenos.Any(d => d.DuenoId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: DuenoController/Delete/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var dueno = await _context.Duenos.FindAsync(id);
            if (dueno == null || dueno.Estado == "Eliminado")
                return NotFound();

            dueno.Estado = "Eliminado";
            dueno.FechaEliminacion = DateTime.Now;
            dueno.UsuarioEliminacion = userId;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
