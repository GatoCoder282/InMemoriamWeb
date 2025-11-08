using InMemoriam.Core.Entities;

namespace InMemoriam.Core.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ExistsEmailAsync(string? email);
        Task<IEnumerable<User>> GetAllDapperAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdDapperAsync(int id);
    }
}
