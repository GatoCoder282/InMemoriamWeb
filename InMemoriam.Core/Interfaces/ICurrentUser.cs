namespace InMemoriam.Core.Interfaces
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        string? IpAddress { get; }
        string? Device { get; }
    }
}
