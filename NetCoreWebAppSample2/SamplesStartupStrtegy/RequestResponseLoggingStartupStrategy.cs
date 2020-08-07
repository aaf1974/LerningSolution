using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAppSample2.RequestResponceLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
