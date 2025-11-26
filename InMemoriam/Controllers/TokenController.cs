using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InMemoriam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public TokenController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Authentication([FromBody] UserLoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Email))
                return BadRequest(new { error = "Email requerido" });

            var user = await _userRepository.GetByEmailAsync(login.Email);
            if (user == null)
                return Unauthorized(new { error = "Credenciales inválidas" });

            if (string.IsNullOrWhiteSpace(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Credenciales inválidas" });
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}