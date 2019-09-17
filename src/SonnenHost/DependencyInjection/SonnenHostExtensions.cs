using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Network;
using PhilipDaubmeier.SonnenHost.Config;
using PhilipDaubmeier.SonnenHost.Database;
using PhilipDaubmeier.SonnenHost.Polling;
using PhilipDaubmeier.SonnenHost.ViewModel;
using PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using System;

namespace PhilipDaubmeier.SonnenHost.DependencyInjection
{
    public static class SonnenHostExtensions
    {
        public static IServiceCollection AddSonnenHost(this IServiceCollection serviceCollection, IConfiguration sonnenConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<SonnenConfig>(sonnenConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<SonnenPortalClient>();

            serviceCollection.AddScoped<ISonnenConnectionProvider, SonnenConfigConnectionProvider>();
            serviceCollection.AddScoped<SonnenPortalClient>();
            serviceCollection.AddPollingService<ISonnenPollingService, SonnenPollingService>();
            serviceCollection.AddTimedPollingHost<ISonnenPollingService>(sonnenConfig.GetSection("PollingService"));

            serviceCollection.AddGraphCollectionViewModel<SonnenEnergyViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddSonnenHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration sonnenConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, ISonnenDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<ISonnenDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddSonnenHost(sonnenConfig, tokenStoreConfig);
        }
    }
}