namespace InMemoriam.Core.Entities
{
    public class FamilyGroup : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<Memorial> Memorials { get; set; } = new List<Memorial>();
    }
}
