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
    public interface IMediaAssetService
    {
        Task<PagedList<MediaAsset>> GetPagedAsync(MediaAssetQueryFilter filter);
        Task<MediaAsset?> GetByIdAsync(int id);
        Task<MediaAsset> CreateAsync(MediaAsset entity);
        Task UpdateAsync(MediaAsset entity);
        Task DeleteAsync(int id);
    }
}
