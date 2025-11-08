namespace InMemoriam.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telephone { get; set; }

        public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
    }
}
