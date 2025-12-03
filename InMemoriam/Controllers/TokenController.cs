using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InMemoriam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

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

        [HttpGet("TestConeccion")]
        public async Task<IActionResult> TestConeccion()
        {
            var result = new
            {
                ConnectionMySql = _configuration["ConnectionStrings:ConnectionMySql"],
                ConnectionSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"]
            };

            return Ok(result);
        }

        [HttpGet("Config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var connectionStringMySql = _configuration["ConnectionStrings:ConnectionMySql"];
                var connectionStringSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"];

                var result = new
                {
                    connectionStringMySql = connectionStringMySql ?? "My SQL NO CONFIGURADO",
                    connectionStringSqlServer = connectionStringSqlServer ?? "SQL SERVER NO CONFIGURADO",
                    AllConnectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren().Select(x => new { Key = x.Key, Value = x.Value }),
                    Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "ASPNETCORE_ENVIRONMENT NO CONFIGURADO",
                    Authentication = _configuration.GetSection("Authentication").GetChildren().Select(x => new { Key = x.Key, Value = x.Value })
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }



    }
}