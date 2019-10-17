using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Netatmo.Config;
using PhilipDaubmeier.GraphIoT.Netatmo.Database;
using PhilipDaubmeier.GraphIoT.Netatmo.Polling;
using PhilipDaubmeier.GraphIoT.Netatmo.Structure;
using PhilipDaubmeier.GraphIoT.Netatmo.ViewModel;
using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

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

            serviceCollection.AddNetatmoHttpClient();
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

        public static IServiceCollection AddNetatmoHttpClient(this IServiceCollection serviceCollection)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(sleepDurations: new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    });

            var timeoutIndividualTryPolicy = Policy
                .TimeoutAsync<HttpResponseMessage>(5);

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromMinutes(2)
                );

            serviceCollection.AddHttpClient<NetatmoConfigConnectionProvider>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1); // Overall timeout across all tries
            })
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}