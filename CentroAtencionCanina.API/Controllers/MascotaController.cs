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
    public class MascotaController : ControllerBase
    {

        private readonly AppDbContext _context;

        public MascotaController(AppDbContext context)
        {
            _context = context;
        }


        // GET: MascaotaController
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = HttpContext.User;
            var query = _context.Mascotas.AsQueryable();
            query = query.Where(m => m.Estado != "Eliminado");
            var mascotas = await query
                .Include(m => m.Raza)
                .Include(m => m.Dueno)
                .Select(m => new MascotaDto
                {
                    MascotaId = m.MascotaId,
                    NombreDueno = m.Dueno.NombreCompleto,
                    NombreMascota = m.NombreMascota,
                    Especie = m.Especie,
                    NombreRaza = m.Raza.NombreRaza,
                    FechaNacimiento = m.FechaNacimiento,
                    Sexo = m.Sexo,
                    Color = m.Color,
                    Peso = m.Peso,
                    InformacionAdicional = m.InformacionAdicional,
                    Estado = m.Estado,
                })
                .ToListAsync();
            return Ok(mascotas);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var query = _context.Mascotas.AsQueryable();

            // Total de registros antes de aplicar filtros o paginación (si es necesario después)
            var total = await query.CountAsync();
            return Ok(total);
        }


        // GET: MascotaController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Mascotas.AsQueryable();
            // El recepcionista NO ve dueños eliminadas
            query = query.Where(m => m.Estado != "Eliminado");

            var mascotas = await query
                .Select(m => new MascotaSelectDto
                {
                    MascotaId = m.MascotaId,
                    NombreMascota = m.NombreMascota,
                })
                .ToListAsync();
            return Ok(mascotas);
        }

        // GET: MascaotaController/5
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var query = _context.Mascotas.AsQueryable();
            var mascota = await query
                .Include(m => m.Raza)
                .Include(m => m.Dueno)
                .Select(m => new MascotaDto
                {
                    MascotaId = m.MascotaId,
                    NombreDueno = m.Dueno.NombreCompleto,
                    NombreMascota = m.NombreMascota,
                    Especie = m.Especie,
                    NombreCategoria = m.Raza.Categoria.NombreCategoria,
                    NombreRaza = m.Raza.NombreRaza,
                    FechaNacimiento = m.FechaNacimiento,
                    Sexo = m.Sexo,
                    Color = m.Color,
                    Peso = m.Peso,
                    InformacionAdicional = m.InformacionAdicional,
                    Estado = m.Estado,
                })
                .FirstOrDefaultAsync(m => m.MascotaId == id);

            if( mascota == null) return NotFound();
            
            return Ok(mascota);
        }

        // GET: MascaotaController/5
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("mascota/{id}")]
        public async Task<IActionResult> GetByMascotaId(int id)
        {
            // Lógica de búsqueda
            var query = _context.Mascotas.AsQueryable();
            var mascota = await query
                .Include(m => m.Raza)
                .Include(m => m.Dueno)
                .Where(m => m.DuenoId == id)
                .Select(m => new UsuarioMascotaDto
                {
                    MascotaId = m.MascotaId,
                    DuenoId = m.DuenoId,
                    NombreMascota = m.NombreMascota,
                    Especie = m.Especie,
                    NombreRaza = m.Raza.NombreRaza,
                    Estado = m.Estado,
                })
                .ToListAsync();

            if (mascota == null || !mascota.Any()) return NotFound();

            return Ok(mascota);
        }

        // POST: MascaotaController/Create
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMascotaDto nuevaMascotaDto)
        {
            var nuevaMascota = new Mascota()
            {
                DuenoId = nuevaMascotaDto.DuenoId,
                NombreMascota = nuevaMascotaDto.NombreMascota,
                Especie = nuevaMascotaDto.Especie,
                RazaId = nuevaMascotaDto.RazaId,
                FechaNacimiento = nuevaMascotaDto.FechaNacimiento,
                Sexo = nuevaMascotaDto.Sexo,
                Color = nuevaMascotaDto.Color,
                Peso = nuevaMascotaDto.Peso,
                InformacionAdicional = nuevaMascotaDto.InformacionAdicional,
                Estado = "Activo",
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = 1,
            };
            _context.Mascotas.Add(nuevaMascota);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevaMascota.MascotaId }, null);
        }

        // PUT: MascaotaController/Edit/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMascotaDto mascotaActualizadaDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var mascota = await _context.Mascotas.FindAsync(id);
                if (mascota == null || mascota.Estado == "Eliminado")
                    return NotFound();

                mascota.DuenoId = mascotaActualizadaDto.DuenoId;
                mascota.NombreMascota = mascotaActualizadaDto.NombreMascota;
                mascota.Especie = mascotaActualizadaDto.Especie;
                mascota.RazaId = mascotaActualizadaDto.RazaId;
                mascota.FechaNacimiento = mascotaActualizadaDto.FechaNacimiento;
                mascota.Sexo = mascotaActualizadaDto.Sexo;
                mascota.Color = mascotaActualizadaDto.Color;
                mascota.Peso = mascotaActualizadaDto.Peso;
                mascota.InformacionAdicional = mascotaActualizadaDto.InformacionAdicional;
                mascota.Estado = mascotaActualizadaDto.Estado;
                mascota.FechaModificacion = DateTime.Now;
                mascota.UsuarioModificacion = userId;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Mascotas.Any(m => m.MascotaId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Delete/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var mascota = await _context.Mascotas.FindAsync(id);
            if (mascota == null || mascota.Estado == "Eliminado")
                return NotFound();

            mascota.Estado = "Eliminado";
            mascota.FechaEliminacion = DateTime.Now;
            mascota.UsuarioEliminacion = userId;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
