using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.QueryFilters;

namespace InMemoriam.Core.Interfaces
{
    public interface IMediaAssetRepository : IBaseRepository<MediaAsset>
    {
        Task<int> CountByMemorialAsync(int memorialId);
        Task<bool> ExistsChecksumAsync(int memorialId, string checksum);
        Task<PagedList<MediaAsset>> GetPaged(MediaAssetQueryFilter filter);
    }
}
