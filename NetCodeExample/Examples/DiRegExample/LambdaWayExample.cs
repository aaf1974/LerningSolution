using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCodeExample.Examples.DiRegExample
{
    class LambdaWayExample
    {
    }

    class StartupDemo3
    {
        void RegisterDemo()
        {
            IServiceCollection services = null;

            services.AddScoped<DiDemoServiceA>();
            services.AddScoped<DiDemoServiceB>();

            services.AddScoped<Func<int, IDiDemoService>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case 1:
                        return serviceProvider.GetService<DiDemoServiceA>();
                    case 2:
                        return serviceProvider.GetService<DiDemoServiceB>();
                    default:
                        throw new Exception("Not valid key");
                }
            });
        }
    }


    public class DiUsingSample2
    {

        private readonly IDiDemoService _service1;

        public DiUsingSample2(Func<int, IDiDemoService> serviceProvider)
        {
            _service1 = serviceProvider(1);
        }
    }
}
