using LinkDev.Talabat.APIs.Errors;
using System.Text.Json;

namespace LinkDev.Talabat.APIs.Middlewares
{
    public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env, RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Take an action with the request
            try
            {
                await next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var response = env.IsDevelopment()
                    ? new ApiExceptionResponse(httpContext.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) 
                    : new ApiExceptionResponse(httpContext.Response.StatusCode);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonResponse = JsonSerializer.Serialize(response);

                await httpContext.Response.WriteAsync(jsonResponse);
            }
            // Take an action with the response
        }
    }
}
