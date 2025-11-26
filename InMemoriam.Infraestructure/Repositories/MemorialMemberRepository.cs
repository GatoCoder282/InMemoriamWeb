using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Queries;

namespace InMemoriam.Infraestructure.Repositories
{
    public class MemorialMemberRepository : IMemorialMemberRepository
    {
        private readonly AppDbContext _db;
        private readonly IDapperContext _dapper;

        public MemorialMemberRepository(AppDbContext db, IDapperContext dapper)
        {
            _db = db;
            _dapper = dapper;
        }

        public Task<IEnumerable<MemorialMember>> GetByMemorialAsync(int memorialId)
            => _dapper.QueryAsync<MemorialMember>(MemorialMemberQueries.GetByMemorial, new { MemorialId = memorialId });

        public Task<MemorialMember?> GetByMemorialAndUserAsync(int memorialId, int userId)
            => _dapper.QueryFirstOrDefaultAsync<MemorialMember>(MemorialMemberQueries.GetByMemorialAndUser, new { MemorialId = memorialId, UserId = userId });

        public async Task Add(MemorialMember m)
        {
            _db.MemorialMembers.Add(m);
            await Task.CompletedTask;
        }

        public async Task Delete(MemorialMember m)
        {
            _db.MemorialMembers.Remove(m);
            await Task.CompletedTask;
        }
    }
}
