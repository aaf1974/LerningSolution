using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace NetCodeExample.Examples.DiRegExample
{
    //https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
    class StartupDemo2
    {
        void RegisterDemo()
        {
            IServiceCollection services = null;

            services.AddSingleton<IDiDemoService, DiDemoServiceA>();
            services.AddSingleton<IDiDemoService, DiDemoServiceB>();
            services.AddSingleton<IDiDemoService, DiDemoServiceC>();
        }

        void UsingDi(IServiceProvider serviceProvider)
        {
            var services = serviceProvider.GetServices<IDiDemoService>();
            var serviceB = services.First(o => o.GetType() == typeof(DiDemoServiceB));
        }
    }
}
