using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace SpotifyAPI.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMiniProfilerWrapper _miniProfilerWrapper;

        public AuditMiddleware(RequestDelegate next, IMiniProfilerWrapper miniProfilerWrapper)
        {
            _next = next;
            _miniProfilerWrapper = miniProfilerWrapper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (_miniProfilerWrapper.Step("AuditMiddleware.InvokeAsync"))
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
