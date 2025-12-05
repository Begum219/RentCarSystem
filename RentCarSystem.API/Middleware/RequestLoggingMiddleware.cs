using System.Diagnostics;

namespace RentCarSystem.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Request bilgileri
            _logger.LogInformation(
                "Request: {Method} {Path} started",
                context.Request.Method,
                context.Request.Path
            );

            await _next(context);

            stopwatch.Stop();

            // Response bilgileri
            _logger.LogInformation(
                "Request: {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode
            );
        }
    }
}