using System;
using CESI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CESI.Extensions
{
    public static class CesiServiceCollectionExtensions
    {
        public static CesiClientBuilder AddCesiClientBuilder(this IServiceCollection services)
        {
            return new CesiClientBuilder(services);
        }
        
        /// <summary>
        /// Injects CesiClient dependencies with no configuration data.
        /// It is implied that you must configure a CesiConfiguration object in your startup.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static CesiClientBuilder AddCesiClient(this IServiceCollection services)
        {
            var builder = services.AddCesiClientBuilder();

            builder
                .AddRequiredPlatformServices()
                .AddCesiCoreServices()
                .AddCesiClientAuthentication()
                .AddCesiEndpoints();

            return builder;
        }

        /// <summary>
        /// Injects CesiClient dependencies with a lambda expression to allow you to manually set configuration in code.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static CesiClientBuilder AddCesiClient(this IServiceCollection services, Action<CesiConfiguration> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.Configure(setupAction);

            return services.AddCesiClient();
        }
        
        /// <summary>
        /// Injects CesiClient dependencies with a configuration parameter which will we automatically add to the DI collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static CesiClientBuilder AddCesiClient(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<CesiConfiguration>(configuration);

            return services.AddCesiClient();
        }
    }
}