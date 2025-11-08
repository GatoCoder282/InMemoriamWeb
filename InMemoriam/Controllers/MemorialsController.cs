using AutoMapper;
using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.QueryFilters;
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
    public sealed class MemorialsController : ControllerBase
    {
        private readonly IMemorialService _service;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;

        public MemorialsController(IMemorialService service, IMapper mapper, IValidatorService validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista paginada de memoriales")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MemorialDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPaged([FromQuery] MemorialQueryFilter filter, CancellationToken ct)
        {
            var paged = await _service.GetPagedAsync(filter);
            var dto = _mapper.Map<IEnumerable<MemorialDto>>(paged.Items);

            var meta = new PaginationMeta
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Ok(ApiResponse<IEnumerable<MemorialDto>>.Success(dto, null, meta));
        }

        [HttpGet("{id:int}", Name = "GetMemorialById")]
        [SwaggerOperation(Summary = "Obtiene un memorial por Id")]
        [ProducesResponseType(typeof(ApiResponse<MemorialDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inválido" }));

            var entity = await _service.GetByIdAsync(id);
            if (entity is null)
                return NotFound(ApiResponse<object>.Fail(new[] { "Memorial no encontrado" }));

            var dto = _mapper.Map<MemorialDto>(entity);
            return Ok(ApiResponse<MemorialDto>.Success(dto));
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Crea un memorial")]
        [ProducesResponseType(typeof(ApiResponse<MemorialDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] MemorialDto dto, CancellationToken ct)
        {
            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.Memorial>(dto);
            var created = await _service.CreateAsync(entity);
            var outDto = _mapper.Map<MemorialDto>(created);

            return CreatedAtRoute("GetMemorialById", new { id = created.Id },
                ApiResponse<MemorialDto>.Success(outDto, "created"));
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Actualiza un memorial")]
        [ProducesResponseType(typeof(ApiResponse<MemorialDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] MemorialDto dto, CancellationToken ct)
        {
            if (id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inválido" }));
            if (dto.Id != 0 && dto.Id != id) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inconsistente" }));

            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.Memorial>(dto);
            entity.Id = id;

            await _service.UpdateAsync(entity);
            var outDto = _mapper.Map<MemorialDto>(entity);
            return Ok(ApiResponse<MemorialDto>.Success(outDto, "updated"));
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Elimina un memorial")]
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
