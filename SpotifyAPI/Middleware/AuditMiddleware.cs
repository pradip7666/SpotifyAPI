using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace SpotifyAPI.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (MiniProfiler.Current.Step("AuditMiddleware.InvokeAsync"))
            {
                var request = context.Request;
                var userName = context.User.Identity?.Name ?? "Anonymous";

                Log.Information("Request {Method} {Url} made by {User}", request.Method, request.Path, userName);

                await _next(context);

                var response = context.Response;
                Log.Information("Response {StatusCode} for {Method} {Url} by {User}", response.StatusCode, request.Method, request.Path, userName);
            }
        }
    }

    public static class AuditMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditMiddleware>();
        }
    }
}
