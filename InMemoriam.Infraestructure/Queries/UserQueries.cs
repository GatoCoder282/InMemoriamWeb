namespace InMemoriam.Infraestructure.Queries
{
    public static class UserQueries
    {
        public const string GetById = @"SELECT Id, FirstName, LastName, Email, PasswordHash, DateOfBirth, IsActive, CreatedAt, UpdatedAt FROM Users WHERE Id=@Id;";
        public const string GetAll = @"SELECT Id, FirstName, LastName, Email, DateOfBirth, IsActive, CreatedAt, UpdatedAt FROM Users ORDER BY Id DESC;";
        public const string GetByEmail = @"SELECT Id, FirstName, LastName, Email, PasswordHash, DateOfBirth, IsActive, CreatedAt, UpdatedAt FROM Users WHERE Email=@Email;";
        public const string ExistsEmail = @"SELECT 1 FROM Users WHERE Email=@Email LIMIT 1;";
    }
}
