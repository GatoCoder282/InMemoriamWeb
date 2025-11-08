using InMemoriam.Core.CustomEntities;
using InMemoriam.Core.Enum;

namespace InMemoriam.Core.QueryFilters
{
    public sealed class MemorialQueryFilter : Pagination
    {
        public string? Search { get; set; }
        public int? OwnerUserId { get; set; }
        public MemorialVisibility? Visibility { get; set; }
        public bool? IsActive { get; set; }
    }
}
