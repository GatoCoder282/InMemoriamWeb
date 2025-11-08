using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Entities;
using InMemoriam.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Core.Interfaces
{
    public interface IMemorialService
    {
        Task<PagedList<Memorial>> GetPagedAsync(MemorialQueryFilter filter);
        Task<Memorial?> GetByIdAsync(int id);
        Task<Memorial> CreateAsync(Memorial entity);
        Task UpdateAsync(Memorial entity);
        Task DeleteAsync(int id);
    }
}
