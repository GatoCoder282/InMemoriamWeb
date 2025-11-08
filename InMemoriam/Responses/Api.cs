namespace InMemoriam.Responses
{
    public static class Api
    {
        public static ApiResponse<T> Ok<T>(T data, string? msg = null, PaginationMeta? meta = null)
        => ApiResponse<T>.Success(data, msg, meta);


        public static ApiResponse<T> Error<T>(object errors, string? msg = null)
        => ApiResponse<T>.Fail(errors, msg);
    }
}
