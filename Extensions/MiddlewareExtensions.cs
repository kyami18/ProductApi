using ProductApi.Middleware;

namespace ProductApi.Extensions;

public static class MiddlewareExtensions
{
    public static void UseApplicationMiddleware(
        this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}