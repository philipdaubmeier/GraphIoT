using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannClient.Model;
using PhilipDaubmeier.ViessmannHost.Config;
using PhilipDaubmeier.ViessmannHost.Database;
using PhilipDaubmeier.ViessmannHost.Polling;
using PhilipDaubmeier.ViessmannHost.ViewModel;
using System;

namespace PhilipDaubmeier.ViessmannHost.DependencyInjection
{
    public static class ViessmannHostExtensions
    {
        public static IServiceCollection AddViessmannHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration viessmannConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, IViessmannDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IViessmannDbContext, TDbContext>(dbConfig);
            
            serviceCollection.Configure<ViessmannConfig>(viessmannConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);
            serviceCollection.AddTokenStore<ViessmannEstrellaClient>();
            serviceCollection.AddTokenStore<ViessmannPlatformClient>();
            serviceCollection.AddTokenStore<ViessmannVitotrolClient>();

            serviceCollection.AddScoped<IViessmannConnectionProvider<ViessmannEstrellaClient>, ViessmannConfigConnectionProvider<ViessmannEstrellaClient>>();
            serviceCollection.AddScoped<IViessmannConnectionProvider<ViessmannPlatformClient>, ViessmannConfigConnectionProvider<ViessmannPlatformClient>>();
            serviceCollection.AddScoped<IViessmannConnectionProvider<ViessmannVitotrolClient>, ViessmannConfigConnectionProvider<ViessmannVitotrolClient>>();

            serviceCollection.AddScoped<ViessmannEstrellaClient>();
            serviceCollection.AddScoped<ViessmannPlatformClient>();
            serviceCollection.AddScoped<ViessmannVitotrolClient>();

            serviceCollection.AddPollingService<IViessmannPollingService, ViessmannSolarPollingService>();
            serviceCollection.AddPollingService<IViessmannPollingService, ViessmannHeatingPollingService>();
            serviceCollection.AddTimedPollingHost<IViessmannPollingService>(viessmannConfig.GetSection("PollingService"));

            serviceCollection.AddGraphCollectionViewModel<ViessmannHeatingViewModel>();
            serviceCollection.AddGraphCollectionViewModel<ViessmannSolarViewModel>();

            return serviceCollection;
        }
    }
}