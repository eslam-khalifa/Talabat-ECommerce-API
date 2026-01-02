namespace LinkDev.Talabat.APIs.Errors
{
    public class ApiSuccessResponse : ApiBaseResponse
    {
        public object? Data { get; set; }

        public ApiSuccessResponse(int statusCode, string? message = null, object? data = null) 
            : base(statusCode, message)
        {
            Data = data;
        }

        public ApiSuccessResponse(int statusCode, object? data) 
            : base(statusCode, null)
        {
            Data = data;
        }
    }
}
