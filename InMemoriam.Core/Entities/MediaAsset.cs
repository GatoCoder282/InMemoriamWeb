using InMemoriam.Core.Enum;

namespace InMemoriam.Core.Entities
{
    public class MediaAsset : BaseEntity
    {
       public MediaAsset() { }
        public int MemorialId { get; set; }
        public Memorial Memorial { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public MediaKind Kind { get; set; } = MediaKind.Photo;
        public string StorageKey { get; set; } = null!; 
        public long SizeBytes { get; set; }
        public string Checksum { get; set; } = null!;

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public string? Tags { get; set; } 
    }
}
