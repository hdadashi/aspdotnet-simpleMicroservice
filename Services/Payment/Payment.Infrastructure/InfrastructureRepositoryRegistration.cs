using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Jobs;
using Payment.Domain.Interfaces;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repository;
using Quartz;

namespace Payment.Infrastructure
{
    public static class InfrastructureRepositoryRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PaymentConnectionString")));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey("ExpirePendingTransactionsJob");
                q.AddJob<ExpirePendingTransactionsJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("ExpirePendingTransactionsTrigger")
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
