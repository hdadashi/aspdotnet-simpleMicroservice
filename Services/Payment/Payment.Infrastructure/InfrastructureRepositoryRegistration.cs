using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Interfaces;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repository;

namespace Payment.Infrastructure
{
    public static class InfrastructureRepositoryRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PaymentConnectionString")));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
