using InMemoriam.Core.Interfaces;

namespace InMemoriam.Infraestructure.Security
{
    public sealed class AmbientCurrentUser : ICurrentUser
    {
        private static readonly AsyncLocal<AmbientCurrentUser?> _ambient = new();

        public static void Set(Guid? userId, string? ip = null, string? device = null)
            => _ambient.Value = new AmbientCurrentUser { UserId = userId, IpAddress = ip, Device = device };

        public static void Clear() => _ambient.Value = null;

        public Guid? UserId { get; private set; }
        public string? IpAddress { get; private set; }
        public string? Device { get; private set; }
    }
}
