namespace InMemoriam.Core.Entities
{
    public class QrCode : BaseEntity
    {
        public QrCode() { }
        public int MemorialId { get; set; }
        public Memorial Memorial { get; set; } = null!;

        public string Serial { get; set; } = null!;   
        public string Token { get; set; } = null!; 
        public DateTime? LastScanAt { get; set; }
    }
}
