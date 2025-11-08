using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace InMemoriam.Infraestructure.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IDapperContext _dapper;
        private IDbContextTransaction? _efTx;


        public UnitOfWork(
        AppDbContext context,
        IDapperContext dapper,
        IMemorialRepository memorialRepo,
        IMediaAssetRepository mediaRepo,
        IUserRepository userRepo)
        {
            _context = context;
            _dapper = dapper;
            MemorialRepository = memorialRepo;
            MediaAssetRepository = mediaRepo;
            UserRepository = userRepo;
        }


        public IMemorialRepository MemorialRepository { get; }
        public IMediaAssetRepository MediaAssetRepository { get; }
        public IUserRepository UserRepository { get; }


        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();


        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            if (_efTx != null) return _efTx.GetDbTransaction();
            _efTx = await _context.Database.BeginTransactionAsync();
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();
            _dapper.SetAmbientConnection(conn, _efTx.GetDbTransaction());
            return _efTx.GetDbTransaction();
        }


        public async Task CommitAsync()
        {
            if (_efTx == null) return;
            await _context.SaveChangesAsync();
            await _efTx.CommitAsync();
            _dapper.ClearAmbientConnection();
            await _efTx.DisposeAsync();
            _efTx = null;
        }


        public async Task RollbackAsync()
        {
            if (_efTx == null) return;
            await _efTx.RollbackAsync();
            _dapper.ClearAmbientConnection();
            await _efTx.DisposeAsync();
            _efTx = null;
        }


        public async ValueTask DisposeAsync()
        {
            if (_efTx != null) await _efTx.DisposeAsync();
        }
    }
}