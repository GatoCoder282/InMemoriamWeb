using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;
using InMemoriam.Core.Enum;
using InMemoriam.Core.Exceptions;
using InMemoriam.Core.Interfaces;

namespace InMemoriam.Core.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IInvitationRepository _invRepo;
        private readonly IMemorialMemberRepository _memberRepo;

        public InvitationService(IUnitOfWork uow)
        {
            _uow = uow;
            _invRepo = (IInvitationRepository)uow.GetType() // fallback: resolve from UoW not available; prefer DI registration
                ;
        }

        // Constructor for DI-friendly usage
        public InvitationService(IUnitOfWork uow, IInvitationRepository invRepo, IMemorialMemberRepository memberRepo)
        {
            _uow = uow;
            _invRepo = invRepo;
            _memberRepo = memberRepo;
        }

        public async Task<Invitation> CreateAsync(Invitation inv)
        {
            if (await _invRepo.ExistsPendingAsync(inv.MemorialId, inv.Email))
                throw new BusinessException("Ya existe una invitación pendiente para este email.");

            inv.Token = Guid.NewGuid().ToString("N");
            inv.Status = InvitationStatus.Pending;
            inv.CreatedAt = DateTime.UtcNow;

            await _invRepo.Add(inv);
            await _uow.SaveChangesAsync();
            return inv;
        }

        public Task<Invitation?> GetByTokenAsync(string token) => _invRepo.GetByTokenAsync(token);

        public async Task<IEnumerable<Invitation>> GetByMemorialAsync(int memorialId) => await _invRepo.GetByMemorialAsync(memorialId);

        public async Task AcceptAsync(string token, int acceptingUserId)
        {
            var inv = await _invRepo.GetByTokenAsync(token) ?? throw new NotFoundException("Invitación no encontrada");
            if (inv.Status != InvitationStatus.Pending) throw new BusinessException("Invitación no válida");
            if (inv.ExpiresAt.HasValue && inv.ExpiresAt.Value < DateTime.UtcNow) throw new BusinessException("Invitación expirada");

            // crear miembro y actualizar invitación dentro de transacción
            await _uow.BeginTransactionAsync();
            try
            {
                var member = new MemorialMember
                {
                    MemorialId = inv.MemorialId,
                    UserId = acceptingUserId,
                    Role = inv.Role,
                    Status = MembershipStatus.Active,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _memberRepo.Add(member);

                inv.Status = InvitationStatus.Accepted;
                inv.AcceptedAt = DateTime.UtcNow;
                await _invRepo.Update(inv);

                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
