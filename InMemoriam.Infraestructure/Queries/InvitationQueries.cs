using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Infraestructure.Queries
{
    public static class InvitationQueries
    {
        public const string GetByToken = @"
            SELECT Id, MemorialId, Email, Role, Token, Status, ExpiresAt, AcceptedAt, IsActive, CreatedAt, UpdatedAt
            FROM Invitations WHERE Token = @Token LIMIT 1;";

        public const string GetById = @"
            SELECT Id, MemorialId, Email, Role, Token, Status, ExpiresAt, AcceptedAt, IsActive, CreatedAt, UpdatedAt
            FROM Invitations WHERE Id = @Id LIMIT 1;";

        public const string GetByMemorial = @"
            SELECT Id, MemorialId, Email, Role, Token, Status, ExpiresAt, AcceptedAt, IsActive, CreatedAt, UpdatedAt
            FROM Invitations WHERE MemorialId = @MemorialId ORDER BY CreatedAt DESC;";

        public const string ExistsPending = @"
            SELECT 1 FROM Invitations WHERE MemorialId = @MemorialId AND Email = @Email AND Status = 0 LIMIT 1;"; // pending = 0

        public const string CountPending = @"
            SELECT COUNT(1) FROM Invitations WHERE MemorialId = @MemorialId AND Email = @Email AND Status = 0;";
    }
}
