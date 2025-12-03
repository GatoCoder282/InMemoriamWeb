using InMemoriam.Core.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoriam.Core.Entities
{
    public class Memorial : BaseEntity
    {
        public Memorial() { }
        public string Slug { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public DateOnly? DeathDate { get; set; }
        public MemorialVisibility Visibility { get; set; } = MemorialVisibility.Private;

        // único campo de almacenamiento en memoria
        private int _ownerId;

        // Propiedad que usa tu API / DTO (OwnerUserId)
        [Column("OwnerUserId")]
        public int OwnerUserId
        {
            get => _ownerId;
            set => _ownerId = value;
        }

        // Propiedad usada por la relación/clave foránea en el modelo EF (OwnerId)
        [ForeignKey(nameof(Owner))]
        [Column("OwnerId")]
        public int OwnerId
        {
            get => _ownerId;
            set => _ownerId = value;
        }

        public User Owner { get; set; } = null!;

        public ICollection<MediaAsset> Media { get; set; } = new List<MediaAsset>();
        public ICollection<Tribute> Tributes { get; set; } = new List<Tribute>();
        public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
        public ICollection<QrCode> QrCodes { get; set; } = new List<QrCode>();
    }
}
