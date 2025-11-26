using AutoMapper;
using BCrypt.Net;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using InMemoriam.Infraestructure.Validators;
using InMemoriam.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InMemoriam.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;
        private readonly ITokenService _tokenService;

        public AuthController(
            IUserService userService,
            IUserRepository userRepository,
            IMapper mapper,
            IValidatorService validator,
            ITokenService tokenService)
        {
            _userService = userService;
            _userRepository = userRepository;
            _mapper = mapper;
            _validator = validator;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Registro de usuario")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserDto dto)
        {
            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(ApiResponse<object>.Fail(new[] { "Password es requerido para creación" }));

            var entity = _mapper.Map<InMemoriam.Core.Entities.User>(dto);

            // Hash de contraseña
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var created = await _userService.CreateAsync(entity);
            var outDto = _mapper.Map<UserDto>(created);

            return CreatedAtRoute("GetUserById", new { id = created.Id },
                ApiResponse<UserDto>.Success(outDto, "created"));
        }

        [HttpPost("login")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Login y obtención de JWT")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto login)
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
            var userDto = _mapper.Map<UserDto>(user);
            // No devolver password ni passwordHash
            return Ok(new { token, user = userDto });
        }
    }
}
