using System.Data;

namespace InMemoriam.Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IMemorialRepository MemorialRepository { get; }
        IMediaAssetRepository MediaAssetRepository { get; }
        IUserRepository UserRepository { get; }

        Task<int> SaveChangesAsync();
        Task<IDbTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
