using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection
{
    public static class TimeseriesHostExtensions
    {
        public static IServiceCollection AddPollingService<TPollingService, TPollingServiceImplementation>(this IServiceCollection serviceCollection) where TPollingService : class, IScopedPollingService where TPollingServiceImplementation : class, TPollingService
        {
            return serviceCollection.AddScoped<TPollingService, TPollingServiceImplementation>();
        }

        public static IServiceCollection AddTimedPollingHost<TPollingService>(this IServiceCollection serviceCollection, IConfiguration config) where TPollingService : IScopedPollingService
        {
            return serviceCollection.Configure<TimedHostedPollingConfig<TPollingService>>(config)
                                    .AddHostedService<TimedHostedPollingService<TPollingService>>();
        }

        public static IServiceCollection AddTimedPollingHost<TPollingService>(this IServiceCollection serviceCollection, string interval, string name) where TPollingService : IScopedPollingService
        {
            return serviceCollection.Configure<TimedHostedPollingConfig<TPollingService>>(options =>
                                    {
                                        options.Name = name;
                                        options.TimerInterval = interval;
                                    })
                                    .AddHostedService<TimedHostedPollingService<TPollingService>>();
        }
    }
}