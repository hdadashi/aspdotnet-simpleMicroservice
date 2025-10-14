using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Features.Interfaces;
using Payment.Infrastructure.Jobs;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Services;
using Quartz;

namespace Payment.Infrastructure
{
    public static class InfrastructureServiceRegistration
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
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}
