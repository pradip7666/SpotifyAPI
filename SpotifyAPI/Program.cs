using Serilog;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using SpotifyAPI.Repository;
using StackExchange.Profiling;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.MapControllers();

app.Run();
