namespace InMemoriam.Core.Entities
{
    public class Condolence : BaseEntity
    {
        public Condolence() { }
        public int TributeId { get; set; }
        public Tribute Tribute { get; set; } = null!;

        public string AuthorNameOrEmail { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
