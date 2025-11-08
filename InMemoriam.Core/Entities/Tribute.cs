namespace InMemoriam.Core.Entities
{
    public class Tribute : BaseEntity
    {
        public Tribute() { }
        public int MemorialId { get; set; }
        public Memorial Memorial { get; set; } = null!;

        public int AuthorUserId { get; set; }
        public User Author { get; set; } = null!;

        public string Body { get; set; } = null!; 
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public ICollection<Condolence> Condolences { get; set; } = new List<Condolence>();
    }
}
