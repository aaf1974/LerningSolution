using FeatureSampe.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace FeatureSampe
{
    /// <summary>
    /// <para>https://andrewlock.net/filtering-action-methods-with-feature-flags-adding-feature-flags-to-an-asp-net-core-app-part-2/</para>
    /// <para>https://stackoverflow.com/questions/50381426/is-there-an-aspnetcore-equivalent-of-the-old-webapi-ihttpcontrollertyperesolver</para>
    /// <para>https://stackoverflow.com/questions/36680933/discovering-generic-controllers-in-asp-net-core</para>
    /// <para>https://stackoverflow.com/questions/68084782/how-to-change-configuration-at-runtime-in-asp-net-core</para>
    /// <para>https://stackoverflow.com/questions/41653688/asp-net-core-appsettings-json-update-in-code</para>
    /// <para>https://stackoverflow.com/questions/61324768/how-can-i-exclude-a-controller-from-asp-net-core-3-attribute-routing</para>
    /// <para>https://docs.microsoft.com/ru-ru/azure/azure-app-configuration/use-feature-flags-dotnet-core?tabs=core5x</para>
    /// </summary>
    public static class FeatureProviderSamples
    {
        public static IServiceCollection Sample(this IServiceCollection services)
        {
            return services;
            /*            services.AddMvc(options => {
                options.Filters.AddForFeature<FeatureFilterMetadata>("Beta");
            });*/

            /*            services.AddMvc()
                                .ConfigureApplicationPartManager(manager =>
                                {
                                    var controllerFeatureProvider = manager.FeatureProviders
                                            .Single(p => p.GetType() == typeof(Microsoft.AspNetCore.Mvc.Controllers.ControllerFeatureProvider));
                                    manager.FeatureProviders[manager.FeatureProviders.IndexOf(controllerFeatureProvider)] =
                                       new Attributes.MyControllerFeatureProvider();
                                });*/

            /*            services
                            .AddMvc()
                            .ConfigureApplicationPartManager(apm =>
                            {
                                var originals = apm.FeatureProviders.OfType<ControllerFeatureProvider>().ToList();
                                foreach (var original in originals)
                                {
                                    apm.FeatureProviders.Remove(original);
                                }
                                apm.FeatureProviders.Add(new SingleControllerFeatureProvider(typeof(AuditSettingController)));
                            });*/

            /*            services.AddControllers()
                            .ConfigureApplicationPartManager(manager =>
                            {
                                manager.FeatureProviders.Remove(manager.FeatureProviders.OfType<ControllerFeatureProvider>().FirstOrDefault());
                                manager.FeatureProviders.Add(new CustomControllerFeatureProvider(configuration));
                            });*/
        }



        public class CustomControllerFeatureProvider : ControllerFeatureProvider
        {
            private readonly IConfiguration _configuration;

            public CustomControllerFeatureProvider(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            protected override bool IsController(TypeInfo typeInfo)
            {
                var isController =
                    base.IsController(typeInfo)
                    && typeInfo == typeof(WeatherForecastController)
                    && true;

                //if (isController && typeInfo.)
                //{
                //    var enabledController = _configuration.GetValue<string[]>("EnabledController");

                //    isController = enabledController.Any(x => typeInfo.Name.Equals(x, StringComparison.InvariantCultureIgnoreCase));
                //}

                return isController;
            }
        }


        public class SingleControllerFeatureProvider : ControllerFeatureProvider
        {
            readonly Type m_type;

            public SingleControllerFeatureProvider(Type type)
            {
                m_type = type;
            }

            protected override bool IsController(TypeInfo typeInfo)
            {
                Console.WriteLine(typeInfo.FullName);
                return false;
                //return base.IsController(typeInfo) && typeInfo == m_type.GetTypeInfo();
            }
        }
    }
}
