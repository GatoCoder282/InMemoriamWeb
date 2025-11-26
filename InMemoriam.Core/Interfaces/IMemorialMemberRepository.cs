using InMemoriam.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Core.Interfaces
{
    public interface IMemorialMemberRepository
    {
        Task<IEnumerable<MemorialMember>> GetByMemorialAsync(int memorialId);
        Task<MemorialMember?> GetByMemorialAndUserAsync(int memorialId, int userId);
        Task Add(MemorialMember m);
        Task Delete(MemorialMember m);
    }
}
