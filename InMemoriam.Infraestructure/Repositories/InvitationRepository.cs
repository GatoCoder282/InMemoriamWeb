using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Queries;

namespace InMemoriam.Infraestructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly AppDbContext _db;
        private readonly IDapperContext _dapper;

        public InvitationRepository(AppDbContext db, IDapperContext dapper)
        {
            _db = db;
            _dapper = dapper;
        }

        public Task<Invitation?> GetByTokenAsync(string token)
            => _dapper.QueryFirstOrDefaultAsync<Invitation>(InvitationQueries.GetByToken, new { Token = token });

        public Task<IEnumerable<Invitation>> GetByMemorialAsync(int memorialId)
            => _dapper.QueryAsync<Invitation>(InvitationQueries.GetByMemorial, new { MemorialId = memorialId });

        public async Task Add(Invitation inv)
        {
            _db.Invitations.Add(inv);
            await Task.CompletedTask;
        }

        public async Task Update(Invitation inv)
        {
            _db.Invitations.Update(inv);
            await Task.CompletedTask;
        }

        public Task<bool> ExistsPendingAsync(int memorialId, string email)
            => _dapper.ExecuteScalarAsync<int>(InvitationQueries.CountPending, new { MemorialId = memorialId, Email = email })
               .ContinueWith(t => t.Result > 0);
    }
}
