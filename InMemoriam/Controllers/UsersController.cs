using AutoMapper;
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
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;

        public UsersController(IUserService service, IMapper mapper, IValidatorService validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista de usuarios")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var items = await _service.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<UserDto>>(items);
            return Ok(ApiResponse<IEnumerable<UserDto>>.Success(dto));
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [SwaggerOperation(Summary = "Obtiene un usuario por Id")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inválido" }));

            var entity = await _service.GetByIdAsync(id);
            if (entity is null)
                return NotFound(ApiResponse<object>.Fail(new[] { "Usuario no encontrado" }));

            var dto = _mapper.Map<UserDto>(entity);
            return Ok(ApiResponse<UserDto>.Success(dto));
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Crea un usuario")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] UserDto dto, CancellationToken ct)
        {
            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.User>(dto);
            var created = await _service.CreateAsync(entity);
            var outDto = _mapper.Map<UserDto>(created);

            return CreatedAtRoute("GetUserById", new { id = created.Id },
                ApiResponse<UserDto>.Success(outDto, "created"));
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Actualiza un usuario")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto, CancellationToken ct)
        {
            if (id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inválido" }));
            if (dto.Id != 0 && dto.Id != id) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inconsistente" }));

            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.User>(dto);
            entity.Id = id;

            await _service.UpdateAsync(entity);
            var outDto = _mapper.Map<UserDto>(entity);
            return Ok(ApiResponse<UserDto>.Success(outDto, "updated"));
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Elimina un usuario")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inválido" }));

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
