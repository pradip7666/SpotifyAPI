using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog;
using SpotifyAPI.Repository;

namespace SpotifyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000, // Set file size limit as needed
                    retainedFileCountLimit: 7) // Retain 7 files before deleting old ones

                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SpotifyAPI", Version = "v1" });
            });

            // Register HttpClient
            builder.Services.AddHttpClient<ISpotifyRepository, SpotifyRepository>();

            // Register services and repositories
            builder.Services.AddScoped<ISpotifyRepository, SpotifyRepository>();

            // Add MiniProfiler
            builder.Services.AddMiniProfiler(options => options.RouteBasePath = "/profiler").AddEntityFramework();

            // Add Health Checks
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotifyAPI v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMiniProfiler();

            // Map health checks and other endpoints
            app.MapHealthChecks("/health");
            app.MapControllers();

            app.Run();
        }
    }
}

