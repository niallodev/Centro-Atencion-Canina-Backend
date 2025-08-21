using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CentroAtencionCanina.API.Services
{
    public class EmailService
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        //private readonly int _smtpPort = 587;
        private readonly int _smtpPort = 465;
        private readonly string _smtpUser = "nloorbazurto@gmail.com"; // tu correo
        private readonly string _smtpPass = "scup oudw cpbr lkpu";

        private readonly string _remitente = "nloorbazurto@gmail.com";

        public async Task EnviarCorreoRecuperacion(string email, string nuevaContrasena)
        {
            // 1. Cargar plantilla HTML
            var rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "passwordReceptor.html");
            var htmlContenido = await File.ReadAllTextAsync(rutaPlantilla);
            htmlContenido = htmlContenido.Replace("{{newPassword}}", nuevaContrasena);

            // 2. Crear mensaje con MimeKit
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Centro de Atención Canina", _remitente));
            mensaje.To.Add(MailboxAddress.Parse(email));
            mensaje.Subject = "Tu nueva contraseña temporal";
            mensaje.Body = new TextPart("html")
            {
                Text = htmlContenido
            };

            // 3. Enviar usando MailKit en puerto 465
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);
        }
    }
}
