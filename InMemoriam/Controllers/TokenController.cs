using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InMemoriam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public TokenController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Authentication([FromBody] UserLoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Email))
                return BadRequest(new { error = "Email requerido" });

            var user = await _userRepository.GetByEmailAsync(login.Email);
            if (user == null)
                return Unauthorized(new { error = "Credenciales inválidas" });

            // Verificar contraseña con BCrypt
            if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Credenciales inválidas" });
            }

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(Core.Entities.User user)
        {
            var secret = _configuration["Authentication:SecretKey"] ?? string.Empty;
            var issuer = _configuration["Authentication:Issuer"];
            var audience = _configuration["Authentication:Audience"];
            var expiresMinutes = int.TryParse(_configuration["Authentication:ExpiresMinutes"], out var em) ? em : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // NameIdentifier
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}