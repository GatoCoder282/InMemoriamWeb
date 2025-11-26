using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;

namespace InMemoriam.Core.Interfaces
{
    public interface IMemorialMemberService
    {
        Task<IEnumerable<MemorialMember>> GetByMemorialAsync(int memorialId);
        Task AddMemberAsync(MemorialMember member);
        Task RemoveMemberAsync(int memorialId, int userId);
    }
}
