namespace InMemoriam.Core.Entities
{
    public class User : BaseEntity
    {
        public User() { }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telephone { get; set; }

        public string? PasswordHash { get; set; }

        public DateOnly? DateOfBirth { get; set; }


        public ICollection<AccessPolicy> AccessPolicies { get; set; } = new List<AccessPolicy>();
    }
}