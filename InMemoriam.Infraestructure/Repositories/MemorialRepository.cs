using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.QueryFilters;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Queries;
using Microsoft.EntityFrameworkCore;

namespace InMemoriam.Infraestructure.Repositories
{
    public class MemorialRepository : BaseRepository<Memorial>, IMemorialRepository
    {
        private readonly IDapperContext _dapper;
        public MemorialRepository(AppDbContext db, IDapperContext dapper) : base(db) { _dapper = dapper; }

        public override Task<Memorial?> GetById(int id)
            => _dapper.QueryFirstOrDefaultAsync<Memorial>(MemorialQueries.GetById, new { Id = id });

        public override Task<IEnumerable<Memorial>> GetAll()
            => _dapper.QueryAsync<Memorial>(MemorialQueries.GetAll);

        public Task<bool> SlugExists(string slug)
            => _dapper.ExecuteScalarAsync<int>(MemorialQueries.ExistsSlug, new { Slug = slug })
               .ContinueWith(t => t.Result == 1);

        public async Task<PagedList<Memorial>> GetPaged(MemorialQueryFilter f)
        {
            var p = new { f.Search, f.OwnerUserId, f.Visibility, f.IsActive, Take = f.PageSize, Skip = (f.PageNumber - 1) * f.PageSize };
            var total = await _dapper.ExecuteScalarAsync<int>(MemorialQueries.CountPaged, p);
            var items = await _dapper.QueryAsync<Memorial>(MemorialQueries.GetPaged, p);
            return new PagedList<Memorial>(items, total, f.PageSize, f.PageNumber);
        }

        public Task<int> CountByOwnerToday(int ownerUserId, DateTime today)
            => _dapper.ExecuteScalarAsync<int>(MemorialQueries.CountByOwnerToday, new { OwnerUserId = ownerUserId, Today = today });
    }

}
