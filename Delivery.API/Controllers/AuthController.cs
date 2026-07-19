using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Delivery.API.Data;
using Delivery_Models;

namespace Delivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DeliveryAPIContext _context;
        private readonly IConfiguration _config;

        public AuthController(DeliveryAPIContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

       
        private static readonly ConcurrentDictionary<string, (string Code, DateTime ExpiresAt)> _pendingCodes = new();
        private static readonly ConcurrentDictionary<string, DateTime> _verifiedEmails = new();

        private static string BuildKey(string email, string purpose) => $"{email.ToLower()}:{purpose}";

        private static string GenerateCode() => new Random().Next(1000, 9999).ToString();


        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"];
            var port = int.Parse(smtp["Port"] ?? "587");
            var username = smtp["Username"];
            var password = smtp["Password"];
            var from = smtp["From"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var message = new MailMessage(from!, toEmail, subject, body);
            message.IsBodyHtml = true;
            await client.SendMailAsync(message);
        }


        public class SendCodeRequest
        {
            public string Email { get; set; } = "";
            public string Purpose { get; set; } = "";
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Purpose))
                return BadRequest("Correo y propósito son requeridos");

            if (request.Purpose == "ResetPassword")
            {
                var exists = await _context.Persons.AnyAsync(p => p.Email == request.Email);
                if (!exists)
                    return NotFound("No existe una cuenta registrada con ese correo");
            }

            if (request.Purpose == "Register")
            {
                var exists = await _context.Persons.AnyAsync(p => p.Email == request.Email);
                if (exists)
                    return BadRequest("Ese correo ya está registrado");
            }

            var code = GenerateCode();
            var key = BuildKey(request.Email, request.Purpose);
            _pendingCodes[key] = (code, DateTime.UtcNow.AddMinutes(10));

            try
            {
                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 480px; margin: auto; padding: 30px; background-color: #FBF6EC; border-radius: 16px; border: 2px solid #14120F;'>
                        <p style='font-size: 11px; letter-spacing: 2px; color: #FF4D2E; font-weight: bold; margin: 0 0 10px 0;'>DELIVERY</p>
                        <h2 style='color: #14120F; margin: 0 0 10px 0;'>Tu código de verificación</h2>
                        <p style='color: #6b6455; font-size: 14px; margin: 0 0 20px 0;'>Usa este código para continuar. Expira en 10 minutos.</p>
                        <div style='background-color: #14120F; color: white; font-size: 32px; font-weight: bold; letter-spacing: 8px; text-align: center; padding: 16px; border-radius: 12px;'>
                            {code}
                        </div>
                        <p style='color: #8A8578; font-size: 12px; margin-top: 20px;'>Si no solicitaste este código, puedes ignorar este correo.</p>
                    </div>";

                await SendEmailAsync(
                    request.Email,
                    "Tu código de verificación — Delivery",
                    htmlBody
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, "No se pudo enviar el correo: " + ex.Message);
            }

            return Ok(new { message = "Código enviado" });
        }

        
        public class VerifyCodeRequest
        {
            public string Email { get; set; } = "";
            public string Purpose { get; set; } = "";
            public string Code { get; set; } = "";
        }

        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var key = BuildKey(request.Email, request.Purpose);

            if (!_pendingCodes.TryGetValue(key, out var entry))
                return BadRequest("Código incorrecto");

            if (entry.ExpiresAt < DateTime.UtcNow)
            {
                _pendingCodes.TryRemove(key, out _);
                return BadRequest("El código expiró, solicita uno nuevo");
            }

            if (entry.Code != request.Code)
                return BadRequest("Código incorrecto");

            // Código correcto: lo marcamos como verificado por 15 minutos y lo invalidamos para reuso
            _pendingCodes.TryRemove(key, out _);
            _verifiedEmails[key] = DateTime.UtcNow.AddMinutes(15);

            return Ok(new { message = "Código verificado" });
        }

        private bool IsVerified(string email, string purpose)
        {
            var key = BuildKey(email, purpose);
            if (_verifiedEmails.TryGetValue(key, out var expiresAt) && expiresAt > DateTime.UtcNow)
                return true;
            return false;
        }

        private void ClearVerified(string email, string purpose)
        {
            _verifiedEmails.TryRemove(BuildKey(email, purpose), out _);
        }

        // ---------------------------------------------------------------
        // POST api/Auth/reset-password
        // ---------------------------------------------------------------
        public class ResetPasswordRequest
        {
            public string Email { get; set; } = "";
            public string NewPassword { get; set; } = "";
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!IsVerified(request.Email, "ResetPassword"))
                return BadRequest("Debes verificar tu código antes de cambiar la contraseña");

            var person = await _context.Persons.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (person == null)
                return NotFound("Correo no encontrado");

            person.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            ClearVerified(request.Email, "ResetPassword");

            return Ok(new { message = "Contraseña actualizada correctamente" });
        }

        // ---------------------------------------------------------------
        // LOGIN (sin cambios respecto a lo que ya tenías)
        // ---------------------------------------------------------------
        public class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var person = await _context.Persons
                .FirstOrDefaultAsync(p => p.Email == request.Email);

            if (person == null)
                return NotFound("Correo no registrado");

            if (!person.VerifyPassword(request.Password))
                return Unauthorized("Contraseña incorrecta");

            // 🎯 RESTRICCIÓN DE SEGURIDAD GLOBAL: Rebotamos instantáneamente cuentas suspendidas
            if (!person.IsActive)
            {
                return BadRequest("Su cuenta ha sido desactivada temporalmente por la administración de la plataforma. Por favor, acérquese a nuestras oficinas para hablar.");
            }

            string role = person switch
            {
                Administrator => "Administrator",
                Client => "Client",
                Delivery_Models.Delivery => "Delivery",
                Restaurant => "Restaurant",
                _ => "Unknown"
            };

            return Ok(new
            {
                userId = person.Id,
                name = person.Name,
                email = person.Email,
                role = role
            });
        }

        // ---------------------------------------------------------------
        // REGISTER — ahora exige que el correo ya esté verificado
        // ---------------------------------------------------------------
        public class RegisterRequest
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
            public System.DateTime DateBirth { get; set; }
            public string Address { get; set; } = "";
            public string Phone { get; set; } = "";
            public bool Gender { get; set; }
            public string Role { get; set; } = "";

            public string? MainAddress { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? License { get; set; }
            public string? CommercialName { get; set; }
            public string? Category { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!IsVerified(request.Email, "Register"))
                return BadRequest("Debes verificar tu correo antes de crear la cuenta");

            var existing = await _context.Persons.AnyAsync(p => p.Email == request.Email);
            if (existing)
                return BadRequest("Ese correo ya está registrado");

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var dateBirthUtc = DateTime.SpecifyKind(request.DateBirth, DateTimeKind.Utc);

            Person? newPerson = request.Role switch
            {
                "Client" => new Client(request.Name, dateBirthUtc, request.Address, request.Email, request.Phone, request.Gender, hash, request.MainAddress ?? "", request.Latitude, request.Longitude),
                "Delivery" => new Delivery_Models.Delivery(request.Name, dateBirthUtc, request.Address, request.Email, request.Phone, request.Gender, hash, request.License ?? ""),
                "Restaurant" => new Restaurant(request.Name, dateBirthUtc, request.Address, request.Email, request.Phone, request.Gender, hash, request.CommercialName ?? "", request.Category ?? ""),
                _ => null
            };

            if (newPerson == null)
                return BadRequest("Rol inválido. Usa: Client, Delivery o Restaurant");

            _context.Persons.Add(newPerson);
            await _context.SaveChangesAsync();

            ClearVerified(request.Email, "Register");

            return Ok(new { userId = newPerson.Id, role = request.Role, message = "Registrado correctamente" });
        }
    }
}
