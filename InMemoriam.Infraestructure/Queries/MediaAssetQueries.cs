namespace InMemoriam.Infraestructure.Queries
{
    public static class MediaAssetQueries
    {
        public const string GetById = @"
            SELECT Id, MemorialId, Title, Description, Kind, StorageUri, SizeBytes, Checksum, Date, Tags, IsActive, CreatedAt, UpdatedAt
            FROM MediaAssets WHERE Id=@Id;";

        public const string CountPaged = @"
            SELECT COUNT(1) FROM MediaAssets
            WHERE MemorialId=@MemorialId
              AND (@Search IS NULL OR Title LIKE CONCAT('%', @Search, '%') OR Description LIKE CONCAT('%', @Search, '%'))
              AND (@From IS NULL OR Date >= @From)
              AND (@To IS NULL OR Date <= @To)
              AND (@Kind IS NULL OR Kind = @Kind);";

        public const string GetPaged = @"
            SELECT Id, MemorialId, Title, Description, Kind, StorageUri, SizeBytes, Checksum, Date, Tags, IsActive, CreatedAt, UpdatedAt
            FROM MediaAssets
            WHERE MemorialId=@MemorialId
              AND (@Search IS NULL OR Title LIKE CONCAT('%', @Search, '%') OR Description LIKE CONCAT('%', @Search, '%'))
              AND (@From IS NULL OR Date >= @From)
              AND (@To IS NULL OR Date <= @To)
              AND (@Kind IS NULL OR Kind = @Kind)
            ORDER BY Date DESC, Id DESC
            LIMIT @Take OFFSET @Skip;";

        public const string ExistsChecksum = @"SELECT 1 FROM MediaAssets WHERE MemorialId=@MemorialId AND Checksum=@Checksum LIMIT 1;";
        public const string CountByMemorial = @"SELECT COUNT(1) FROM MediaAssets WHERE MemorialId=@MemorialId;";
    }
}
