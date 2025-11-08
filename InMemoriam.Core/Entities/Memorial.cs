using InMemoriam.Core.Enum;

namespace InMemoriam.Core.Entities
{
    public class Memorial : BaseEntity
    {
        public string Slug { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public DateOnly? DeathDate { get; set; }
        public MemorialVisibility Visibility { get; set; } = MemorialVisibility.Private;

        public int OwnerUserId { get; set; }
        public User Owner { get; set; } = null!;

        public ICollection<MediaAsset> Media { get; set; } = new List<MediaAsset>();
        public ICollection<Tribute> Tributes { get; set; } = new List<Tribute>();
        public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
        public ICollection<QrCode> QrCodes { get; set; } = new List<QrCode>();
    }
}
