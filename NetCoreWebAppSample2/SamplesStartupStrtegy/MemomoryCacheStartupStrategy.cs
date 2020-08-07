using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAppSample2.MemoryCacheSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    public class MemomoryCacheStartupStrategy : StartupStrategyBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MemoryCacheService>();
        }

        public override void Configure_Middleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }
    }
}
