using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Interfaces;

namespace InMemoriam.Core.Services
{
    public class MemorialMemberService : IMemorialMemberService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMemorialMemberRepository _repo;

        public MemorialMemberService(IUnitOfWork uow, IMemorialMemberRepository repo)
        {
            _uow = uow;
            _repo = repo;
        }

        public Task<IEnumerable<MemorialMember>> GetByMemorialAsync(int memorialId) => _repo.GetByMemorialAsync(memorialId);

        public async Task AddMemberAsync(MemorialMember member)
        {
            await _repo.Add(member);
            await _uow.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int memorialId, int userId)
        {
            var m = await _repo.GetByMemorialAndUserAsync(memorialId, userId)
                ?? throw new Exception("Member not found");
            await _repo.Delete(m);
            await _uow.SaveChangesAsync();
        }
    }
}
