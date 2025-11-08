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
    [Route("api/memorials/{memorialId:int}/[controller]")]
    [Produces("application/json")]
    public sealed class MediaAssetsController : ControllerBase
    {
        private readonly IMediaAssetService _service;
        private readonly IMapper _mapper;
        private readonly IValidatorService _validator;

        public MediaAssetsController(IMediaAssetService service, IMapper mapper, IValidatorService validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista paginada de media assets de un memorial")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MediaAssetDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPaged(int memorialId, [FromQuery] MediaAssetQueryFilter filter, CancellationToken ct)
        {
            if (memorialId <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inválido" }));

            filter.MemorialId = memorialId;
            var paged = await _service.GetPagedAsync(filter);
            var dto = _mapper.Map<IEnumerable<MediaAssetDto>>(paged.Items);

            var meta = new PaginationMeta
            {
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };

            return Ok(ApiResponse<IEnumerable<MediaAssetDto>>.Success(dto, null, meta));
        }

        [HttpGet("{id:int}", Name = "GetMediaAssetById")]
        [SwaggerOperation(Summary = "Obtiene un media asset por Id")]
        [ProducesResponseType(typeof(ApiResponse<MediaAssetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(int memorialId, int id, CancellationToken ct)
        {
            if (memorialId <= 0 || id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Ids inválidos" }));

            var entity = await _service.GetByIdAsync(id);
            if (entity is null || entity.MemorialId != memorialId)
                return NotFound(ApiResponse<object>.Fail(new[] { "MediaAsset no encontrado" }));

            var dto = _mapper.Map<MediaAssetDto>(entity);
            return Ok(ApiResponse<MediaAssetDto>.Success(dto));
        }

        [HttpPost]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Crea un media asset")]
        [ProducesResponseType(typeof(ApiResponse<MediaAssetDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(int memorialId, [FromBody] MediaAssetDto dto, CancellationToken ct)
        {
            if (memorialId <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inválido" }));

            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            if (dto.MemorialId != 0 && dto.MemorialId != memorialId)
                return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inconsistente" }));

            var entity = _mapper.Map<InMemoriam.Core.Entities.MediaAsset>(dto);
            entity.MemorialId = memorialId;

            var created = await _service.CreateAsync(entity);
            var outDto = _mapper.Map<MediaAssetDto>(created);

            return CreatedAtRoute("GetMediaAssetById",
                new { memorialId, id = created.Id },
                ApiResponse<MediaAssetDto>.Success(outDto, "created"));
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Actualiza un media asset")]
        [ProducesResponseType(typeof(ApiResponse<MediaAssetDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(int memorialId, int id, [FromBody] MediaAssetDto dto, CancellationToken ct)
        {
            if (memorialId <= 0 || id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Ids inválidos" }));
            if (dto.Id != 0 && dto.Id != id) return BadRequest(ApiResponse<object>.Fail(new[] { "Id inconsistente" }));
            if (dto.MemorialId != 0 && dto.MemorialId != memorialId)
                return BadRequest(ApiResponse<object>.Fail(new[] { "MemorialId inconsistente" }));

            var vr = await _validator.ValidateAsync(dto);
            if (!vr.IsValid) return BadRequest(new { Message = "Error de validación", Errors = vr.Errors });

            var entity = _mapper.Map<InMemoriam.Core.Entities.MediaAsset>(dto);
            entity.Id = id;
            entity.MemorialId = memorialId;

            await _service.UpdateAsync(entity);
            var outDto = _mapper.Map<MediaAssetDto>(entity);
            return Ok(ApiResponse<MediaAssetDto>.Success(outDto, "updated"));
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Elimina un media asset")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int memorialId, int id, CancellationToken ct)
        {
            if (memorialId <= 0 || id <= 0) return BadRequest(ApiResponse<object>.Fail(new[] { "Ids inválidos" }));

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
