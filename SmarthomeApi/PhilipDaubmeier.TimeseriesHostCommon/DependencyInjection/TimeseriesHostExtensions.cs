using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.ViewModel;

namespace PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection
{
    public static class TimeseriesHostExtensions
    {
        public static IServiceCollection AddGraphCollectionViewModel<TGraphCollectionViewModelImplementation>(this IServiceCollection serviceCollection) where TGraphCollectionViewModelImplementation : class, IGraphCollectionViewModel
        {
            return serviceCollection.AddGraphCollectionViewModel<IGraphCollectionViewModel, TGraphCollectionViewModelImplementation>();
        }

        public static IServiceCollection AddGraphCollectionViewModel<TGraphCollectionViewModel, TGraphCollectionViewModelImplementation>(this IServiceCollection serviceCollection) where TGraphCollectionViewModel : class, IGraphCollectionViewModel where TGraphCollectionViewModelImplementation : class, TGraphCollectionViewModel
        {
            return serviceCollection.AddScoped<TGraphCollectionViewModel, TGraphCollectionViewModelImplementation>();
        }

        public static IServiceCollection AddDatabaseBackupService<TDbContext>(this IServiceCollection serviceCollection) where TDbContext : DbContext
        {
            return serviceCollection.AddScoped<DatabaseBackupService<TDbContext>>();
        }

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