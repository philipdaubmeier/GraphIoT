using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.GraphIoT.Core.Config;
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
        public static IServiceCollection AddNetatmoHost(this IServiceCollection serviceCollection, IConfiguration netatmoConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig)
        {
            serviceCollection.Configure<NetatmoConfig>(netatmoConfig);

            serviceCollection.Configure<NetworkConfig>(networkConfig);

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

        public static IServiceCollection AddNetatmoHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration netatmoConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig) where TDbContext : DbContext, INetatmoDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<INetatmoDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddNetatmoHost(netatmoConfig, tokenStoreConfig, networkConfig);
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

            serviceCollection.AddHttpClient<NetatmoHttpClient>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1); // Overall timeout across all tries
            })
                .ConfigurePrimaryHttpMessageHandler(services => new HttpClientHandler().SetProxy(services.GetService<IOptions<NetworkConfig>>()))
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}