using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAppSample2.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.SamplesStartupStrtegy
{
    public class BackgroundTaskSratupStrategy : StartupStrategyBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<TimedHostedService>();

            //ScopedService
            services.AddHostedService<ConsumeScopedServiceHostedService>();
            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

            //QueueBackground
            services.AddSingleton<MonitorLoop>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
