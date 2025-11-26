
using System;
using InMemoriam.Core.Enum;

namespace InMemoriam.Core.Entities
{
    public class MemorialMember : BaseEntity
    {
        public MemorialMember() { }

        public int MemorialId { get; set; }
        public Memorial? Memorial { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public AccessRole Role { get; set; } = AccessRole.Viewer;
        public MembershipStatus Status { get; set; } = MembershipStatus.Active;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
