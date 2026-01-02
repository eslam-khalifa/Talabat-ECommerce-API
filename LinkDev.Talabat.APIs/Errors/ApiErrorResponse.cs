namespace LinkDev.Talabat.APIs.Errors
{
    public class ApiErrorResponse : ApiBaseResponse
    {
        public IReadOnlyList<string> Errors { get; set; }

        public ApiErrorResponse(int statusCode, string errorMessage) 
            : base(statusCode, null)
        {
            Errors = new List<string> { errorMessage };
        }

        public ApiErrorResponse(int statusCode, IReadOnlyList<string> errors) 
            : base(statusCode, null)
        {
            Errors = errors;
        }

        public ApiErrorResponse(int statusCode, string? message, IReadOnlyList<string> errors) 
            : base(statusCode, message)
        {
            Errors = errors;
        }
    }
}
