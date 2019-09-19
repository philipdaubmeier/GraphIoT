using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.GraphIoT.Netatmo.Config;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Polling;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.GraphIoT.Netatmo.ViewModel;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using System;

namespace PhilipDaubmeier.GraphIoT.Netatmo.DependencyInjection
{
    public static class NetatmoHostExtensions
    {
        public static IServiceCollection AddNetatmoHost(this IServiceCollection serviceCollection, IConfiguration netatmoConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<NetatmoConfig>(netatmoConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<NetatmoWebClient>();

            serviceCollection.AddSingleton<INetatmoDeviceService, NetatmoDeviceService>();

            serviceCollection.AddScoped<INetatmoConnectionProvider, NetatmoConfigConnectionProvider>();
            serviceCollection.AddScoped<NetatmoWebClient>();
            serviceCollection.AddPollingService<INetatmoPollingService, NetatmoWeatherPollingService>();
            serviceCollection.AddTimedPollingHost<INetatmoPollingService>(netatmoConfig.GetSection("PollingService"));

            serviceCollection.AddGraphCollectionViewModel<NetatmoMeasureViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddNetatmoHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration netatmoConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, INetatmoDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<INetatmoDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddNetatmoHost(netatmoConfig, tokenStoreConfig);
        }
    }
}