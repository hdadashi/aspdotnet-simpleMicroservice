using Microsoft.Extensions.DependencyInjection;
using Payment.Application.EventBus;

namespace Payment.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IMessagePublisher, MessagePublisher>();

            return services;
        }
    }
}
