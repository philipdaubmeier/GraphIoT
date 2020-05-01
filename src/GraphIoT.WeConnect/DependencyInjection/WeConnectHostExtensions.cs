using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.GraphIoT.Core.Config;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.WeConnect.Config;
using PhilipDaubmeier.GraphIoT.WeConnect.Database;
using PhilipDaubmeier.GraphIoT.WeConnect.Polling;
using PhilipDaubmeier.GraphIoT.WeConnect.ViewModel;
using PhilipDaubmeier.WeConnectClient;
using PhilipDaubmeier.WeConnectClient.Network;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.WeConnect.DependencyInjection
{
    public static class WeConnectHostExtensions
    {
        public static IServiceCollection AddWeConnectHost(this IServiceCollection serviceCollection, IConfiguration weConnectConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig)
        {
            serviceCollection.Configure<WeConnectConfig>(weConnectConfig);

            serviceCollection.Configure<NetworkConfig>(networkConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<WeConnectPortalClient>();

            serviceCollection.AddWeConnectHttpClient<WeConnectHttpClient>();
            serviceCollection.AddWeConnectHttpClient<WeConnectAuthHttpClient>();
            serviceCollection.AddScoped<IWeConnectConnectionProvider, WeConnectConfigConnectionProvider>();
            serviceCollection.AddScoped<WeConnectPortalClient>();

            serviceCollection.AddPollingService<IWeConnectPollingService, WeConnectPollingService>();
            serviceCollection.AddTimedPollingHost<IWeConnectPollingService>(weConnectConfig.GetSection("PollingService"));

            serviceCollection.AddGraphCollectionViewModel<WeConnectViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddWeConnectHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration weConnectConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig) where TDbContext : DbContext, IWeConnectDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IWeConnectDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddWeConnectHost(weConnectConfig, tokenStoreConfig, networkConfig);
        }

        public static IServiceCollection AddWeConnectHttpClient<TClient>(this IServiceCollection serviceCollection) where TClient : class
        {
            var useAuthHandler = typeof(TClient) == typeof(WeConnectAuthHttpClient);

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

            serviceCollection.AddHttpClient<TClient>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1); // Overall timeout across all tries
            })
                .ConfigurePrimaryHttpMessageHandler(services =>
                    (useAuthHandler ? WeConnectConnectionProvider.CreateAuthHandler() : WeConnectConnectionProvider.CreateHandler())
                    .SetProxy(services.GetService<IOptions<NetworkConfig>>())
                )
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}