using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.Queries
{
    public static class MemorialMemberQueries
    {
        public const string GetByMemorial = @"
            SELECT Id, MemorialId, UserId, Role, Status, JoinedAt, IsActive, CreatedAt, UpdatedAt
            FROM MemorialMembers WHERE MemorialId = @MemorialId ORDER BY JoinedAt DESC;";

        public const string GetByMemorialAndUser = @"
            SELECT Id, MemorialId, UserId, Role, Status, JoinedAt, IsActive, CreatedAt, UpdatedAt
            FROM MemorialMembers WHERE MemorialId = @MemorialId AND UserId = @UserId LIMIT 1;";
    }
}
