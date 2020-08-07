using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    class PipelineStartupStrategy : StartupStrategyBase
    {
        private List<StartupStrategyBase> elements = new List<StartupStrategyBase>();

        public PipelineStartupStrategy Register(StartupStrategyBase step)
        {
            elements.Add(step);
            return this;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            elements.ForEach(x => x.ConfigureServices(services));
        }

        public override void Configure_Middleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
            elements.ForEach(x => x.Configure_Middleware(app, env));
        }
    }

    class PipelineUsing
    {
        void sample1()
        {
            var sp = new PipelineStartupStrategy()
                .Register(new RequestResponseLoggingStartupStrategy())
                .Register(new RequestResponseLoggingStartupStrategy());


        }
    }
}
