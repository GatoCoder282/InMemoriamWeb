namespace InMemoriam.Infraestructure.Queries
{
    public static class MemorialQueries
    {
        public const string GetById = @"
            SELECT Id, Slug, FullName, BirthDate, DeathDate, Visibility, OwnerUserId, IsActive, CreatedAt, UpdatedAt
            FROM Memorials WHERE Id=@Id;";

        public const string GetAll = @"
            SELECT Id, Slug, FullName, BirthDate, DeathDate, Visibility, OwnerUserId, IsActive, CreatedAt, UpdatedAt
            FROM Memorials ORDER BY Id DESC;";

        public const string CountPaged = @"
            SELECT COUNT(1) FROM Memorials
            WHERE (@Search IS NULL OR FullName LIKE CONCAT('%', @Search, '%') OR Slug LIKE CONCAT('%', @Search, '%'))
              AND (@OwnerUserId IS NULL OR OwnerUserId = @OwnerUserId)
              AND (@Visibility IS NULL OR Visibility = @Visibility)
              AND (@IsActive IS NULL OR IsActive = @IsActive);";

        public const string GetPaged = @"
            SELECT Id, Slug, FullName, BirthDate, DeathDate, Visibility, OwnerUserId, IsActive, CreatedAt, UpdatedAt
            FROM Memorials
            WHERE (@Search IS NULL OR FullName LIKE CONCAT('%', @Search, '%') OR Slug LIKE CONCAT('%', @Search, '%'))
              AND (@OwnerUserId IS NULL OR OwnerUserId = @OwnerUserId)
              AND (@Visibility IS NULL OR Visibility = @Visibility)
              AND (@IsActive IS NULL OR IsActive = @IsActive)
            ORDER BY Id DESC
            LIMIT @Take OFFSET @Skip;";

        public const string CountByOwnerToday = @"
            SELECT COUNT(1) FROM Memorials
            WHERE OwnerUserId=@OwnerUserId AND CAST(CreatedAt AS DATE) = CAST(@Today AS DATE);";

        public const string ExistsSlug = @"SELECT 1 FROM Memorials WHERE Slug=@Slug LIMIT 1;";
    }
}
