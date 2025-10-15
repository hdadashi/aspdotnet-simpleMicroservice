using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.EventBus;
using Payment.Application.Jobs;
using Quartz;

namespace Payment.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
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
            services.AddSingleton<IMessagePublisher, MessagePublisher>();

            return services;
        }

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
