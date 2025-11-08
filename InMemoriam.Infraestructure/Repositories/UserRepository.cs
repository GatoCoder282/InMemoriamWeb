using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.Data;
using InMemoriam.Infraestructure.Queries;
using Microsoft.EntityFrameworkCore;

namespace InMemoriam.Infraestructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IDapperContext _dapper;
        public UserRepository(AppDbContext db, IDapperContext dapper) : base(db) { _dapper = dapper; }

        public Task<User?> GetByIdDapperAsync(int id)
            => _dapper.QueryFirstOrDefaultAsync<User>(UserQueries.GetById, new { Id = id });

        public Task<IEnumerable<User>> GetAllDapperAsync()
            => _dapper.QueryAsync<User>(UserQueries.GetAll);

        public Task<User?> GetByEmailAsync(string email)
            => _dapper.QueryFirstOrDefaultAsync<User>(UserQueries.GetByEmail, new { Email = email });

        public Task<bool> ExistsEmailAsync(string email)
            => _dapper.ExecuteScalarAsync<int>(UserQueries.ExistsEmail, new { Email = email })
               .ContinueWith(t => t.Result == 1);
    }

}
