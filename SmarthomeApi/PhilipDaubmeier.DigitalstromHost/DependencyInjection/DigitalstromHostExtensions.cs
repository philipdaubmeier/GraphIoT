using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromHost.Config;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.EventProcessing;
using PhilipDaubmeier.DigitalstromHost.Polling;
using PhilipDaubmeier.DigitalstromHost.Structure;
using PhilipDaubmeier.DigitalstromHost.ViewModel;
using PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using System;

namespace PhilipDaubmeier.DigitalstromHost.DependencyInjection
{
    public static class DigitalstromHostExtensions
    {
        public static IServiceCollection AddDigitalstromHost(this IServiceCollection serviceCollection, IConfiguration digitalstromConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<DigitalstromConfig>(digitalstromConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<PersistingDigitalstromAuth>();

            serviceCollection.AddTransient<IDigitalstromConnectionProvider, DigitalstromConfigConnectionProvider>();
            serviceCollection.AddScoped<DigitalstromDssClient>();

            serviceCollection.AddSingleton<IDigitalstromStructureService, DigitalstromStructureService>();

            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromEnergyPollingService>();
            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromSensorPollingService>();
            serviceCollection.AddTimedPollingHost<IDigitalstromPollingService>(digitalstromConfig.GetSection("PollingService"));
            serviceCollection.Configure<DigitalstromEventProcessingConfig>(digitalstromConfig.GetSection("EventProcessor"));

            serviceCollection.AddScoped<IDigitalstromEventProcessorPlugin, DssSceneEventProcessorPlugin>();
            serviceCollection.AddHostedService<DigitalstromEventsHostedService>();

            serviceCollection.AddGraphCollectionViewModel<DigitalstromEnergyViewModel>();
            serviceCollection.AddGraphCollectionViewModel<DigitalstromZoneSensorViewModel>();
            serviceCollection.AddEventCollectionViewModel<DigitalstromSceneEventViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddDigitalstromHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration digitalstromConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, IDigitalstromDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IDigitalstromDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddDigitalstromHost(digitalstromConfig, tokenStoreConfig);
        }
    }
}