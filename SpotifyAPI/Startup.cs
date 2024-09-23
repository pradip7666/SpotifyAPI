using SpotifyAPI.Middleware;
using StackExchange.Profiling;

namespace SpotifyAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SpotifyAPI", Version = "v1" });
            });

            // Register the IMiniProfilerWrapper implementation
            services.AddSingleton<IMiniProfilerWrapper, MiniProfilerWrapper>();

            services.AddMiniProfiler(options => options.RouteBasePath = "/profiler").AddEntityFramework();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiniProfiler();
            app.UseAuditMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotifyAPI V1");
                c.RoutePrefix = string.Empty; // This sets Swagger UI at the root of the application.
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

