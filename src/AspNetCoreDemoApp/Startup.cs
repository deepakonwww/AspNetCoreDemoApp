using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace AspNetCoreDemoApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddHttpsRedirection(options => { options.HttpsPort = 443; })
                .AddMvcCore()
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                });

            // Register our Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AspNet Core 5.0 Demo API",
                    Version = "v1"
                });
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO")))
            {
                Console.WriteLine("Use https redirection");
                app.UseHttpsRedirection();
            }

            app
                .UseRouting()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseCors("CorsPolicy")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
        }
    }
}