using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Core.QueryFilters;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Queries;
using Microsoft.EntityFrameworkCore;

namespace InMemoriam.Infraestructure.Repositories
{
    public class MediaAssetRepository : BaseRepository<MediaAsset>, IMediaAssetRepository
    {
        private readonly IDapperContext _dapper;
        public MediaAssetRepository(AppDbContext db, IDapperContext dapper) : base(db) { _dapper = dapper; }

        public override Task<MediaAsset?> GetById(int id)
            => _dapper.QueryFirstOrDefaultAsync<MediaAsset>(MediaAssetQueries.GetById, new { Id = id });

        public async Task<PagedList<MediaAsset>> GetPaged(MediaAssetQueryFilter f)
        {
            var p = new { f.MemorialId, f.Search, From = f.From, To = f.To, f.Kind, Take = f.PageSize, Skip = (f.PageNumber - 1) * f.PageSize };
            var total = await _dapper.ExecuteScalarAsync<int>(MediaAssetQueries.CountPaged, p);
            var items = await _dapper.QueryAsync<MediaAsset>(MediaAssetQueries.GetPaged, p);
            return new PagedList<MediaAsset>(items, total, f.PageSize, f.PageNumber);
        }

        public Task<bool> ExistsChecksumAsync(int memorialId, string checksum)
            => _dapper.ExecuteScalarAsync<int>(MediaAssetQueries.ExistsChecksum, new { MemorialId = memorialId, Checksum = checksum })
               .ContinueWith(t => t.Result == 1);

        public Task<int> CountByMemorialAsync(int memorialId)
            => _dapper.ExecuteScalarAsync<int>(MediaAssetQueries.CountByMemorial, new { MemorialId = memorialId });
    }


}
