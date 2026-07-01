namespace Booking.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    _logger.LogWarning("Business error: {Message} | {Method} {Path}",
                        ex.Message,
                        context.Request.Method,
                        context.Request.Path);
                }
                else
                {
                    _logger.LogError(ex, "Unhandled server exception occurred while processing {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);
                }
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
           HttpContext context,
           Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                statusCode,
                message = exception.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
