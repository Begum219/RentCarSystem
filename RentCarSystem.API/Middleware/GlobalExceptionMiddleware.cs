using Serilog;
using System.Net;

namespace RentCarSystem.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Serilog ile loglama
                Log.Error(ex, " Unhandled exception occurred - Path: {Path}, Method: {Method}, User: {User}",
                    context.Request.Path,
                    context.Request.Method,
                    context.User?.Identity?.Name ?? "Anonymous");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Farklı exception türlerine göre özelleştirilmiş mesajlar
            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Yetkisiz erişim."),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "İstenen kayıt bulunamadı."),
                ArgumentException => (StatusCodes.Status400BadRequest, "Geçersiz parametre."),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Geçersiz işlem."),
                _ => (StatusCodes.Status500InternalServerError, "İsteğin sırasında bir hata oluştu.")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = statusCode,
                message = message,
                details = exception.Message,
                // ✅ Development ortamında stack trace göster
#if DEBUG
                stackTrace = exception.StackTrace
#endif
            };

            // ✅ Farklı seviyelerde loglama (exception parametresini kullan)
            if (statusCode >= 500)
            {
                Log.Error(exception, " Server Error {StatusCode}: {Message}", statusCode, exception.Message);
            }
            else if (statusCode >= 400)
            {
                Log.Warning(" Client Error {StatusCode}: {Message}", statusCode, exception.Message);
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}