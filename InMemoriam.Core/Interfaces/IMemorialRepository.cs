using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.QueryFilters;

namespace InMemoriam.Core.Interfaces
{
    public interface IMemorialRepository : IBaseRepository<Memorial>
    {
        Task<int> CountByOwnerToday(int ownerUserId, DateTime utcNow);
        Task<PagedList<Memorial>> GetPaged(MemorialQueryFilter f);
        Task<bool> SlugExists(string slug);
    }
}
