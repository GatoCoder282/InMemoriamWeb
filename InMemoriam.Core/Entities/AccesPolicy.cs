using InMemoriam.Core.Enum;

namespace InMemoriam.Core.Entities
{
    public class AccessPolicy : BaseEntity
    {
        public int MemorialId { get; set; }
        public Memorial Memorial { get; set; } = null!;

        public int? UserId { get; set; }
        public User? User { get; set; }

        public AccessRole Role { get; set; } = AccessRole.Viewer;
        public DateTime? ExpiresAt { get; set; }
    }
}
