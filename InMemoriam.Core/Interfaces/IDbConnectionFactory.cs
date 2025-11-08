using InMemoriam.Core.Enum;
using System.Data;

namespace InMemoriam.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        DatabaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
