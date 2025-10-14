using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    Assembly.GetExecutingAssembly()
                );
            });

            return services;
        }
    }
}
