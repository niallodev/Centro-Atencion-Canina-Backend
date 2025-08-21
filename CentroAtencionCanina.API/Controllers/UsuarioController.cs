using CentroAtencionCanina.API.Data;
using CentroAtencionCanina.API.DTOs;
using CentroAtencionCanina.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CentroAtencionCanina.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace CentroAtencionCanina.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordService _passwordService;
        private readonly EmailService _emailService;


        public UsuarioController(AppDbContext context, IConfiguration config, PasswordService passwordService, EmailService emailService)
        {
            _context = context;
            _config = config;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        // GET: UsuarioController
        //[Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = _context.Usuarios.AsQueryable();
            var usuarios = await query
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.NombreCompleto,
                    NombreUsuario = u.NombreUsuario,
                    Rol = u.Rol,
                    IntentosFallidos = u.IntentosFallidos,
                    FechaUltimoIntento = u.FechaUltimoIntento.ToString(),
                    Estado = u.Estado,
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        // GET: UsuarioController
        //[Authorize(Roles = "")]
        [HttpGet("select")]
        public async Task<IActionResult> GetAllSelect()
        {
            var user = HttpContext.User;
            var query = _context.Usuarios.AsQueryable();
            query = query.Where(u => u.Estado != "Eliminado");

            var usuarios = await query
                .Select(u => new UsuarioSelectDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.NombreCompleto,
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        // GET: UsuarioController/1
        //[Authorize(Roles = "Administrador")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Lógica de búsqueda
            var query = _context.Usuarios.AsQueryable();
            var usuario = await query
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    NombreCompleto = u.NombreCompleto,
                    NombreUsuario = u.NombreUsuario,
                    Rol = u.Rol,
                    IntentosFallidos = u.IntentosFallidos,
                    FechaUltimoIntento = u.FechaUltimoIntento.ToString(),
                    Estado = u.Estado,
                })    
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuario == null) return NotFound();

            return Ok(usuario);
        }

        // POST: UsuarioController/Create
        [Authorize(Roles = "Administrador,Recepcionista")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUsuarioDto nuevoUsuarioDto)
        {
            var nuevoUsuario = new Usuario
            {
                NombreCompleto = nuevoUsuarioDto.NombreCompleto,
                NombreUsuario = nuevoUsuarioDto.NombreUsuario,
                ContrasenaHash = nuevoUsuarioDto.Contrasenia,
                Rol = nuevoUsuarioDto.Rol,
                IntentosFallidos = 0,
                FechaUltimoIntento = null,
                Estado = "Activo",
                UsuarioCreacion = 1,
            };
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Aquí guardarías en la base de datos usando EF
            return CreatedAtAction(nameof(GetById), new { id = nuevoUsuario.UsuarioId }, null);
        }
        
        // POST: api/usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>u.NombreUsuario == loginDto.NombreUsuario);
            
            if (usuario == null)
                return Unauthorized("Usuario no encontrado");
            if (usuario.Estado == "Bloqueado")
                return Unauthorized("Cuenta bloqueada por múltiples intentos fallidos.");



            // Comparar contraseñas (texto plano contra hash)
            bool passwordValida = _passwordService.VerificarContrasena(usuario.ContrasenaHash, loginDto.Contrasenia);
            if (!passwordValida)
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= 3)
                {
                    usuario.Estado = "Bloqueado";
                }
                await _context.SaveChangesAsync();
                return Unauthorized("Credenciales inválidas");
            }
            
            // Login exitoso: resetear intentos fallidos
            usuario.IntentosFallidos = 0;

            // 2. Generar claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };
            // 3. Generar clave y token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                //expires: DateTime.Now.AddSeconds(15),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 4. Guardar el token en la base
            usuario.Token = tokenString;
            usuario.FechaUltimoIntento = DateTime.Now;
            await _context.SaveChangesAsync();

            // 5. Retornar token + info
            return Ok(new
            {
                Token = tokenString,
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
            });
        }

        // POST: api/usuarios/logout/id
        [HttpPost("logout/{id}")]
        public async Task<IActionResult> Logout(int id)
        {

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return Unauthorized("Usuario no encontrado");
          
            usuario.Token = null;
            await _context.SaveChangesAsync();

            // 5. Retornar token + info
            return Ok();
        }

        //PUT: api/usuarios/recuperar/email
        [HttpPut("recuperar/{email}")]
        public async Task<IActionResult> RecuperarContrasena(string email)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario == null || usuario.Estado == "Eliminado")
                return NotFound("Usuario no encontrado o eliminado.");

            // 1. Generar contraseña nueva
            var nuevaContrasena = _passwordService.GenerarContrasenaAleatoria();

            // 2. Hashear la nueva contraseña
            usuario.ContrasenaHash = _passwordService.HashearContrasena(nuevaContrasena);
            await _context.SaveChangesAsync();

            // 3. Enviar correo con la nueva contraseña
            await _emailService.EnviarCorreoRecuperacion(usuario.Email, nuevaContrasena);

            return Ok("Contraseña recuperada y enviada al correo.");

        }

        // PUT: UsuarioController/Edit/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto usuarioActualizadaDto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null || usuario.Estado == "Eliminado")
                    return NotFound();

                usuario.NombreCompleto = usuarioActualizadaDto.NombreCompleto;
                usuario.NombreUsuario = usuarioActualizadaDto.NombreUsuario;
                usuario.Rol = usuarioActualizadaDto.Rol;
                usuario.Estado = usuarioActualizadaDto.Estado;
                usuario.FechaModificacion = DateTime.Now;
                usuario.UsuarioModificacion = 1;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(u => u.UsuarioId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: UsuarioController/Delete/5
        [Authorize(Roles = "Administrador, Recepcionista")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null || usuario.Estado == "Eliminado")
                return NotFound();

            usuario.Estado = "Eliminado";
            usuario.FechaEliminacion = DateTime.Now;
            usuario.UsuarioEliminacion = 1;

            await _context.SaveChangesAsync();

            return NoContent();
        }

      


}


}

