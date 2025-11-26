using AutoMapper;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.DTOs;
using InMemoriam.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace InMemoriam.Controllers
{
    [ApiController]
    [Route("api/memorials/{memorialId:int}/[controller]")]
    [Produces("application/json")]
    public class MemorialMembersController : ControllerBase
    {
        private readonly IMemorialMemberService _service;
        private readonly IMapper _mapper;

        public MemorialMembersController(IMemorialMemberService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista de miembros de un memorial")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MemorialMemberDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(int memorialId)
        {
            if (memorialId <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inválido" }));
            var items = await _service.GetByMemorialAsync(memorialId);
            var dto = _mapper.Map<IEnumerable<MemorialMemberDto>>(items);
            return Ok(ApiResponse<IEnumerable<MemorialMemberDto>>.Success(dto));
        }

        [HttpDelete("{userId:int}")]
        [SwaggerOperation(Summary = "Remueve un miembro")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Remove(int memorialId, int userId)
        {
            if (memorialId <= 0 || userId <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Ids inválidos" }));
            await _service.RemoveMemberAsync(memorialId, userId);
            return NoContent();
        }
    }
}
