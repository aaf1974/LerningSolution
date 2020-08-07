using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    //https://docs.microsoft.com/ru-ru/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1
    public class ResponceCachingSampleStartupStrategy : StartupStrategyBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //services.AddResponseCaching();

            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1024;
                options.UseCaseSensitivePaths = true;
            });
        }

        public override void Configure_Middleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // UseCors must be called before UseResponseCaching
            // app.UseCors("myAllowSpecificOrigins");

            app.UseResponseCaching();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(60),
                        
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

                await next();
            });
        }
    }
}
