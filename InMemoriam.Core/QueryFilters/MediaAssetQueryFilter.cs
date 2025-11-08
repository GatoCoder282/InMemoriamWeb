using InMemoriam.Core.CustomEntities;

namespace InMemoriam.Core.QueryFilters
{
    public class MediaAssetQueryFilter : Pagination
    {
        public int MemorialId { get; set; }
        public string? Search { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
        public string? Kind { get; set; }   
    }
}
