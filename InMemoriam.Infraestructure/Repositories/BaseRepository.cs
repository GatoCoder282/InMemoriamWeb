using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;
using InMemoriam.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InMemoriam.Infraestructure.Repositories
{
    public  class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _db;
        public BaseRepository(AppDbContext db) => _db = db;

        public virtual async Task<T?> GetById(int id) => await _db.Set<T>().FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAll() => await _db.Set<T>().AsNoTracking().ToListAsync();
        public async Task Add(T entity) => await _db.Set<T>().AddAsync(entity);
        public Task Update(T entity) { _db.Set<T>().Update(entity); return Task.CompletedTask; }
        public async Task Delete(int id)
        {
            var e = await _db.Set<T>().FindAsync(id);
            if (e != null) _db.Set<T>().Remove(e);
        }
    }
}
