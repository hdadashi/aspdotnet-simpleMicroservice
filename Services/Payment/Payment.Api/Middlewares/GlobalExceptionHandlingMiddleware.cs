using System.Net;
using System.Text.Json;

namespace Payment.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    isSuccess = false,
                    message = "خطای داخلی در سرور رخ داده است.",
                    error = ex.Message // لازم به ذکر است که در محیط production بهتر است حذف یا محدود شود
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}