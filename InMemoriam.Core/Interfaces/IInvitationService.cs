using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;

namespace InMemoriam.Core.Interfaces
{
    public interface IInvitationService
    {
        Task<Invitation> CreateAsync(Invitation inv);
        Task<Invitation?> GetByTokenAsync(string token);
        Task AcceptAsync(string token, int acceptingUserId);
        Task<IEnumerable<Invitation>> GetByMemorialAsync(int memorialId);
    }
}
