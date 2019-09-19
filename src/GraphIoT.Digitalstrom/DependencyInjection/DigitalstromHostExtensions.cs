using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Config;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.EventProcessing;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Polling;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using System;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection
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