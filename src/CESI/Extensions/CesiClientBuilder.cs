using System;
using Microsoft.Extensions.DependencyInjection;

namespace CESI.Extensions
{
    public class CesiClientBuilder
    {
        public CesiClientBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}