using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Enum;
using InMemoriam.Core.Exceptions;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Core.Services
{
    public sealed class MediaAssetService : IMediaAssetService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMediaAssetRepository _repo;
        private readonly IMemorialRepository _memRepo;
        private const long MaxSize = 50L * 1024 * 1024; 
        private static readonly HashSet<MediaKind> AllowedKinds =
    new() { MediaKind.Photo, MediaKind.Video, MediaKind.Audio };

        public MediaAssetService(IUnitOfWork uow) { _uow = uow; _repo = uow.MediaAssetRepository; _memRepo = uow.MemorialRepository; }

        public Task<PagedList<MediaAsset>> GetPagedAsync(MediaAssetQueryFilter f) => _repo.GetPaged(f);
        public Task<MediaAsset?> GetByIdAsync(int id) => _repo.GetById(id);

        public async Task<MediaAsset> CreateAsync(MediaAsset e)
        {
            if (await _memRepo.GetById(e.MemorialId) is null) throw new NotFoundException("Memorial no encontrado");
            if (e.SizeBytes > MaxSize) throw new BusinessException("Archivo demasiado grande");
            if (!AllowedKinds.Contains(e.Kind)) throw new BusinessException("Tipo de asset no permitido");
            if (await _repo.ExistsChecksumAsync(e.MemorialId, e.Checksum)) throw new BusinessException("Duplicado por checksum");

            var cnt = await _repo.CountByMemorialAsync(e.MemorialId);
            if (cnt >= 500) throw new BusinessException("Límite de assets alcanzado");

            await _repo.Add(e);
            await _uow.SaveChangesAsync();
            return e;
        }

        public async Task UpdateAsync(MediaAsset e)
        {
            if (await _repo.GetById(e.Id) is null) throw new NotFoundException("MediaAsset no encontrado");
            if (e.SizeBytes > MaxSize) throw new BusinessException("Archivo demasiado grande");
            await _repo.Update(e);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (await _repo.GetById(id) is null) throw new NotFoundException("MediaAsset no encontrado");
            await _repo.Delete(id);
            await _uow.SaveChangesAsync();
        }
    }

}
