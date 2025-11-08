namespace InMemoriam.Responses
{
    public sealed class PaginationMeta
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(PageSize, 1));
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }


    public class ApiResponse<T>
    {
        public string Message { get; init; } = "ok";
        public T? Data { get; init; }
        public PaginationMeta? Pagination { get; init; }
        public object? Errors { get; init; }


        public static ApiResponse<T> Success(T data, string? message = null, PaginationMeta? meta = null)
        => new() { Data = data, Message = message ?? "ok", Pagination = meta };


        public static ApiResponse<T> Fail(object errors, string? message = null)
        => new() { Errors = errors, Message = message ?? "error" };
    }
}
