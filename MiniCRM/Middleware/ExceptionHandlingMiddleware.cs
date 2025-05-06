using System.Net;
using System.Text.Json;

namespace MiniCRM.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var error = new
                {
                    status = context.Response.StatusCode,
                    message = "An unexpected Error has occured.",
                    detail = ex.Message
                };

                var errorJson = JsonSerializer.Serialize(error);
                await context.Response.WriteAsync(errorJson);
            }
        }
    }
}
