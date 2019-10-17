using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Viessmann.Config;
using PhilipDaubmeier.GraphIoT.Viessmann.Database;
using PhilipDaubmeier.GraphIoT.Viessmann.Polling;
using PhilipDaubmeier.GraphIoT.Viessmann.ViewModel;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using PhilipDaubmeier.ViessmannClient;
using PhilipDaubmeier.ViessmannClient.Model;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Viessmann.DependencyInjection
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

            serviceCollection.AddViessmannHttpClient<ViessmannAuthClientProvider>();
            serviceCollection.AddViessmannHttpClient<ViessmannConfigConnectionProvider<ViessmannEstrellaClient>>();
            serviceCollection.AddViessmannHttpClient<ViessmannConfigConnectionProvider<ViessmannPlatformClient>>();
            serviceCollection.AddViessmannHttpClient<ViessmannConfigConnectionProvider<ViessmannVitotrolClient>>();

            serviceCollection.AddScoped<ViessmannAuthClientProvider>();
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

        public static IServiceCollection AddViessmannHttpClient<TClient>(this IServiceCollection serviceCollection) where TClient : class
        {
            var useAuthHandler = typeof(TClient) == typeof(ViessmannAuthClientProvider);

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
                .ConfigurePrimaryHttpMessageHandler(() => useAuthHandler ? ViessmannConnectionProvider<TClient>.CreateAuthHandler() : new HttpClientHandler())
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}