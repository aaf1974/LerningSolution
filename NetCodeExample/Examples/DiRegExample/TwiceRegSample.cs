using Microsoft.Extensions.DependencyInjection;
using NetCoreLibrary;
using System;

namespace NetCodeExample.Examples.DiRegExample
{
    class TwiceRegSample
    {
        internal static void PrintConectedSample()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<IDemoTwiceRegInterface, DemoImplFirst>();
            services.AddScoped<IDemoTwiceRegInterface, DemoImplSecond>();

            ServiceProvider _provider = services.BuildServiceProvider();

            var instance = _provider.GetRequiredService<IDemoTwiceRegInterface>();

            CodeConsole.WriteLineColor(instance.HoAIm, ConsoleColor.Blue, ConsoleColor.White);
        }
    }


    interface IDemoTwiceRegInterface
    {
        string HoAIm { get; }
    }

    class DemoImplFirst : IDemoTwiceRegInterface
    {
        public string HoAIm => this.GetType().FullName;
    }

    class DemoImplSecond : IDemoTwiceRegInterface
    {
        public string HoAIm => this.GetType().FullName;
    }
}
