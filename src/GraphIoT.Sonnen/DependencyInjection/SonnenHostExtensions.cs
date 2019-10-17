using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Sonnen.Config;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using PhilipDaubmeier.GraphIoT.Sonnen.Polling;
using PhilipDaubmeier.GraphIoT.Sonnen.ViewModel;
using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Network;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Sonnen.DependencyInjection
{
    public static class SonnenHostExtensions
    {
        public static IServiceCollection AddSonnenHost(this IServiceCollection serviceCollection, IConfiguration sonnenConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<SonnenConfig>(sonnenConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<SonnenPortalClient>();

            serviceCollection.AddSonnenHttpClient();
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

        public static IServiceCollection AddSonnenHttpClient(this IServiceCollection serviceCollection)
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

            serviceCollection.AddHttpClient<SonnenConfigConnectionProvider>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(1); // Overall timeout across all tries
            })
                .ConfigurePrimaryHttpMessageHandler(() => SonnenConnectionProvider.CreateHandler())
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}