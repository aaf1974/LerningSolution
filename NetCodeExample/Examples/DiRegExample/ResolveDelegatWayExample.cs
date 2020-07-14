using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace NetCodeExample.Examples.DiRegExample
{
    //https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
    public class DiUsingSample
    {
        private readonly IDiDemoService _aService;

        public DiUsingSample(ServiceResolver serviceAccessor)
        {
            _aService = serviceAccessor("A");
        }

        public void UseServiceA()
        {
            _aService.DoTheThing();
        }
    }


    public delegate IDiDemoService ServiceResolver(string key);

    class StartupDemo
    {
        void RegisterDemo()
        {
            IServiceCollection services = null;
            services.AddTransient<DiDemoServiceA>();
            services.AddTransient<DiDemoServiceB>();
            services.AddTransient<DiDemoServiceC>();


            services.AddTransient<ServiceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "A":
                        return serviceProvider.GetService<DiDemoServiceA>();
                    case "B":
                        return serviceProvider.GetService<DiDemoServiceB>();
                    case "C":
                        return serviceProvider.GetService<DiDemoServiceC>();
                    default:
                        throw new KeyNotFoundException(); // or maybe return null, up to you
                }
            });
        }
    }



}
