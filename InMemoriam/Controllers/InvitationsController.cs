using AutoMapper;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using InMemoriam.Infraestructure.Validators;
using InMemoriam.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InMemoriam.Controllers
{
    [ApiController]
    [Route("api/memorials/{memorialId:int}/[controller]")]
    [Produces("application/json")]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _service;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;

        public InvitationsController(IInvitationService service, IMapper mapper, IValidatorService validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Crea una invitación para sumar miembros")]
        [ProducesResponseType(typeof(ApiResponse<InvitationDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(int memorialId, [FromBody] InvitationCreateDto dto)
        {
            if (memorialId <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inválido" }));
            if (dto.MemorialId != 0 && dto.MemorialId != memorialId)
                return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inconsistente" }));

            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.Invitation>(dto);
            entity.MemorialId = memorialId;
            if (!string.IsNullOrWhiteSpace(dto.ExpiresAt) && DateTime.TryParse(dto.ExpiresAt, out var ex))
                entity.ExpiresAt = ex;

            var created = await _service.CreateAsync(entity);
            var outDto = _mapper.Map<InvitationDto>(created);
            return Created(string.Empty, ApiResponse<InvitationDto>.Success(outDto, "created"));
        }

        [HttpPost("accept")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Aceptar invitación (por token)")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Accept([FromQuery] string token, [FromBody] Dictionary<string, int>? body = null)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest(ApiResponse<object>.Fail(new[] { "Token requerido" }));

            // simplifico: el id del usuario que acepta se envía en el body { "userId": 123 }
            if (body == null || !body.TryGetValue("userId", out var userId) || userId <= 0)
                return BadRequest(ApiResponse<object>.Fail(new[] { "userId requerido en body" }));

            await _service.AcceptAsync(token, userId);
            return Ok(ApiResponse<object>.Success(null, "accepted"));
        }
    }
}
