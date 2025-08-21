using CentroAtencionCanina.API.Data;
using CentroAtencionCanina.API.DTOs;
using CentroAtencionCanina.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentroAtencionCanina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RazaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RazaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RazaController
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = _context.Razas.AsQueryable();
            var razas = await query
                .Include(r => r.Categoria)
                .Select(r => new RazaDto
                {
                    RazaId = r.RazaId,
                    NombreRaza = r.NombreRaza,
                    NombreCategoria = r.Categoria.NombreCategoria,
                    Origen = r.Origen,
                    Tamano = r.Tamano,
                    EsperanzaVidaAnios = r.EsperanzaVidaAnios,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado,
                })
                .ToListAsync();
            return Ok(razas);
        }

        // GET: DuenoController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Razas.AsQueryable();
            // El recepcionista NO ve dueños eliminadas
            query = query.Where(d => d.Estado != "Eliminado");

            var razas = await query
                .Select(r => new RazaSelectDto
                {
                    RazaId = r.RazaId,
                    NombreRaza = r.NombreRaza,
                    CategoriaId = r.CategoriaId,
                })
                .ToListAsync();
            return Ok(razas);
        }


        // GET: RazaController/1
        //[Authorize(Roles = "Administrador, Recepcionista")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var query = _context.Razas.AsQueryable();
            var raza = await query
                .Include(r => r.Categoria)
                .Select(r => new RazaDto
                {
                    RazaId = r.RazaId,
                    NombreRaza = r.NombreRaza,
                    NombreCategoria = r.Categoria.NombreCategoria,
                    Origen = r.Origen,
                    Tamano = r.Tamano,
                    EsperanzaVidaAnios = r.EsperanzaVidaAnios,
                    Descripcion = r.Descripcion,
                    Estado = r.Estado,
                })
                .FirstOrDefaultAsync(s => s.RazaId == id);
            
            if (raza == null) return NotFound();

            return Ok(raza);
        }

        // POST: RazaController/Create
        //[Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRazaDto nuevaRazaDto)
        {
            var nuevaRaza = new Raza()
            {
                NombreRaza = nuevaRazaDto.NombreRaza,
                CategoriaId = nuevaRazaDto.CategoriaId,
                Origen = nuevaRazaDto.Origen,
                Tamano = nuevaRazaDto.Tamano,
                EsperanzaVidaAnios = nuevaRazaDto.EsperanzaVidaAnios,
                Descripcion = nuevaRazaDto.Descripcion,
                Estado = "Activo",
                UsuarioCreacion = 1,
            };
            _context.Razas.Add(nuevaRaza);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevaRaza.RazaId }, null);
        }


        // PUT: RazaController/Edit/5
        //[Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRazaDto razaActualizadaDto)
        {
            try
            {
                var raza = await _context.Razas.FindAsync(id);
                if (raza == null || raza.Estado == "Eliminado")
                    return NotFound();

                raza.NombreRaza = razaActualizadaDto.NombreRaza;
                raza.CategoriaId = razaActualizadaDto.CategoriaId;
                raza.Origen = razaActualizadaDto.Origen;
                raza.Tamano = razaActualizadaDto.Tamano;
                raza.EsperanzaVidaAnios = razaActualizadaDto.EsperanzaVidaAnios;
                raza.Descripcion = razaActualizadaDto.Descripcion;
                raza.Estado = razaActualizadaDto.Estado;
                raza.FechaModificacion = DateTime.Now;
                raza.UsuarioModificacion = 1;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Razas.Any(r => r.RazaId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: RazaController/Delete/5
        //[Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var raza = await _context.Razas.FindAsync(id);
            if (raza == null || raza.Estado == "Eliminado")
                return NotFound();

            raza.Estado = "Eliminado";
            raza.FechaModificacion = DateTime.Now;
            raza.UsuarioModificacion = 1;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
