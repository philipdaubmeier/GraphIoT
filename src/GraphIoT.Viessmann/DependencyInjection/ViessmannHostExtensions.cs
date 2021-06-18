using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PhilipDaubmeier.GraphIoT.Core.Config;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Viessmann.Config;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.GraphIoT.Viessmann.Polling;
using PhilipDaubmeier.GraphIoT.Viessmann.ViewModel;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannClient.Network;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.DependencyInjection
{
    public static class ViessmannHostExtensions
    {
        public static IServiceCollection AddViessmannHost(this IServiceCollection serviceCollection, IConfiguration viessmannConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig)
        {
            serviceCollection.Configure<ViessmannConfig>(viessmannConfig);

            serviceCollection.Configure<NetworkConfig>(networkConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<ViessmannPlatformClient>();

            serviceCollection.AddViessmannHttpClient<ViessmannAuthHttpClient>();
            serviceCollection.AddViessmannHttpClient<ViessmannHttpClient<ViessmannPlatformClient>>();

            serviceCollection.AddScoped<IViessmannConnectionProvider<ViessmannPlatformClient>, ViessmannConfigConnectionProvider<ViessmannPlatformClient>>();
            serviceCollection.AddScoped<ViessmannPlatformClient>();

            serviceCollection.AddPollingService<IViessmannPollingService, ViessmannHeatingPollingService>();
            serviceCollection.AddTimedPollingHost<IViessmannPollingService>(viessmannConfig.GetSection("PollingService"));

            serviceCollection.AddGraphCollectionViewModel<ViessmannHeatingViewModel>();
            serviceCollection.AddGraphCollectionViewModel<ViessmannSolarViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddViessmannHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration viessmannConfig, IConfiguration tokenStoreConfig, IConfiguration networkConfig) where TDbContext : DbContext, IViessmannDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IViessmannDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddViessmannHost(viessmannConfig, tokenStoreConfig, networkConfig);
        }

        public static IServiceCollection AddViessmannHttpClient<TClient>(this IServiceCollection serviceCollection) where TClient : class
        {
            var useAuthHandler = typeof(TClient) == typeof(ViessmannAuthHttpClient);

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
                    (useAuthHandler ? ViessmannConnectionProvider<TClient>.CreateAuthHandler() : new HttpClientHandler())
                    .SetProxy(services.GetService<IOptions<NetworkConfig>>())
                )
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}