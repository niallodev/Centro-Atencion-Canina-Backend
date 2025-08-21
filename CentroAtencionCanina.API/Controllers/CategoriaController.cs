using CentroAtencionCanina.API.Data;
using CentroAtencionCanina.API.DTOs;
using CentroAtencionCanina.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CentroAtencionCanina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaController
        //[Authorize(Roles = "Administrador,Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = HttpContext.User;
            var esAdmin = user.IsInRole("Administrador");
            var query = _context.Categorias.AsQueryable();

            if (!esAdmin)
            {
                // El recepcionista NO ve categorías eliminadas
                query = query.Where(c => c.Estado != "Eliminado");
            }

            var categorias = await query
                .Select(c => new CategoriaDto
                {
                    CategoriaId = c.CategoriaId,
                    NombreCategoria = c.NombreCategoria,
                    Descripcion = c.Descripcion,
                    Estado = c.Estado,
                })
                .ToListAsync();
            return Ok(categorias);
        }

        // GET: DuenoController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Categorias.AsQueryable();
            // El recepcionista NO ve dueños eliminadas
            query = query.Where(d => d.Estado != "Eliminado");

            var categorias = await query
                .Select(r => new CategoriaSelectDto
                {
                    CategoriaId = r.CategoriaId,
                    NombreCategoria = r.NombreCategoria,
                })
                .ToListAsync();
            return Ok(categorias);
        }

        // GET: CategoriaController/1
        //[Authorize(Roles = "Administrador,Recepcionista")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var user = HttpContext.User;
            var esAdmin = user.IsInRole("Administrador");
            var query = _context.Categorias.AsQueryable();

            if (!esAdmin)
            {
                // El recepcionista NO ve categorías eliminadas
                query = query.Where(c => c.Estado != "Eliminado");
            }
            var categoria = await query
                .Select(c => new CategoriaDto
                {
                    CategoriaId = c.CategoriaId,
                    NombreCategoria = c.NombreCategoria,
                    Descripcion = c.Descripcion,
                    Estado = c.Estado,
                })
                .FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null) return NotFound();

            return Ok(categoria);
        }

        // POST: CategoriaController/Create
        //[Authorize(Roles = "Administrador,Recepcionista")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaDto nuevaCategoriaDto)
        {
            var nuevaCategoria = new Categoria
            {
                NombreCategoria = nuevaCategoriaDto.NombreCategoria,
                Descripcion = nuevaCategoriaDto.Descripcion,
                Estado = "Activo",
                UsuarioCreacion = 1
            };
            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevaCategoria.CategoriaId }, null);
        }


        // PUT: CategoriaController/Edit/5
        //[Authorize(Roles = "Administrador,Recepcionista")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaDto categoriaActualizadaDto)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null || categoria.Estado == "Eliminado")
                    return NotFound();

                categoria.NombreCategoria = categoriaActualizadaDto.NombreCategoria;
                categoria.Descripcion = categoriaActualizadaDto.Descripcion;
                categoria.Estado = categoriaActualizadaDto.Estado;
                categoria.FechaModificacion = DateTime.Now;
                categoria.UsuarioModificacion = 1;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categorias.Any(c => c.CategoriaId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: CategoriaController/Delete/5
        //[Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null || categoria.Estado == "Eliminado")
                return NotFound();

            categoria.Estado = "Eliminado";
            categoria.FechaEliminacion = DateTime.Now;
            categoria.UsuarioEliminacion = 1;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
