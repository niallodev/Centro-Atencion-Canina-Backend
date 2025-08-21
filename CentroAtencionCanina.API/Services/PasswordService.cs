using Microsoft.AspNetCore.Identity;

namespace CentroAtencionCanina.API.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public string GenerarContrasenaAleatoria(int longitud = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, longitud)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string HashearContrasena(string contrasena)
        {
            return _passwordHasher.HashPassword(null, contrasena);
        }

        public bool VerificarContrasena(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

    }
}
