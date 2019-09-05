using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.NetatmoHost.Config;
using PhilipDaubmeier.NetatmoHost.Polling;
using PhilipDaubmeier.TimeseriesHostCommon.DependencyInjection;
using PhilipDaubmeier.TokenStore.DependencyInjection;

namespace PhilipDaubmeier.NetatmoHost.DependencyInjection
{
    public static class NetatmoHostExtensions
    {
        public static IServiceCollection AddNetatmoHost(this IServiceCollection serviceCollection, IConfiguration netatmoConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<NetatmoConfig>(netatmoConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<NetatmoWebClient>();

            serviceCollection.AddScoped<INetatmoConnectionProvider, NetatmoConfigConnectionProvider>();
            serviceCollection.AddScoped<NetatmoWebClient>();
            serviceCollection.AddPollingService<INetatmoPollingService, NetatmoWeatherPollingService>();
            serviceCollection.AddTimedPollingHost<INetatmoPollingService>(netatmoConfig.GetSection("PollingService"));

            return serviceCollection;
        }
    }
}