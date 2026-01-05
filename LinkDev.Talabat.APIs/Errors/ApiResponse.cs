namespace LinkDev.Talabat.APIs.Errors
{
    [Obsolete("Use ApiSuccessResponse or ApiErrorResponse instead")]
    public class ApiResponse : ApiBaseResponse
    {
        public IReadOnlyList<string>? Errors { get; set; }

        public ApiResponse(int statusCode, string? message = null) 
            : base(statusCode, message)
        {
        }

        public ApiResponse(int statusCode, IReadOnlyList<string> errors) 
            : base(statusCode, null)
        {
            Errors = errors;
        }
    }
}
