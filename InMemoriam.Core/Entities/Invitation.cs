using System;
using InMemoriam.Core.Enum;

namespace InMemoriam.Core.Entities
{
    public class Invitation : BaseEntity
    {
        public Invitation() { }

        public int MemorialId { get; set; }
        public Memorial? Memorial { get; set; }

        public string Email { get; set; } = null!;
        public AccessRole Role { get; set; } = AccessRole.Viewer;

        public string Token { get; set; } = null!;
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;

        public DateTime? ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
