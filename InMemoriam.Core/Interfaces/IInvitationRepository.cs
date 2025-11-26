using InMemoriam.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Core.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation?> GetByTokenAsync(string token);
        Task<IEnumerable<Invitation>> GetByMemorialAsync(int memorialId);
        Task Add(Invitation inv);
        Task Update(Invitation inv);
        Task<bool> ExistsPendingAsync(int memorialId, string email);
    }
}
