using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    public abstract class StartupStrategyBase
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }


        public virtual void Configure_Middleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
