using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAppSample2.RequestResponceLogging;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    public class RequestResponseLoggingStartupStrategy : StartupStrategyBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IResponceRequestLogger, ResponceRequestLoggerDefaultImpl>();
        }

        public override void Configure_Middleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Add our new middleware to the pipeline
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
