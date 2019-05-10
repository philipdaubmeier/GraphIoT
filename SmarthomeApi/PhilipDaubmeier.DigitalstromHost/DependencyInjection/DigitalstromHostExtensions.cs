using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.DigitalstromHost.Config;
using PhilipDaubmeier.DigitalstromHost.Database;
using PhilipDaubmeier.DigitalstromHost.EventProcessing;
using PhilipDaubmeier.DigitalstromHost.Polling;
using PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using System;

namespace PhilipDaubmeier.DigitalstromHost.DependencyInjection
{
    public static class DigitalstromHostExtensions
    {
        public static IServiceCollection AddDigitalstromHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration digitalstromConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, IDigitalstromDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IDigitalstromDbContext, TDbContext>(dbConfig);

            serviceCollection.Configure<DigitalstromConfig>(digitalstromConfig);

            serviceCollection.ConfigureTokenStore<TDbContext>(dbConfig, tokenStoreConfig);
            serviceCollection.AddTokenStore<PersistingDigitalstromAuth>();
            
            serviceCollection.AddTransient<IDigitalstromConnectionProvider, DigitalstromConfigConnectionProvider>();
            serviceCollection.AddScoped<DigitalstromDssClient>();
            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromEnergyPollingService>();
            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromSensorPollingService>();
            serviceCollection.AddTimedPollingHost<IDigitalstromPollingService>(digitalstromConfig.GetSection("PollingService"));

            serviceCollection.AddScoped<IDigitalstromEventProcessorPlugin, DssSceneEventProcessorPlugin>();
            serviceCollection.AddHostedService<DigitalstromEventsHostedService>();
            return serviceCollection;
        }
    }
}