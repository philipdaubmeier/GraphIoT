using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Config;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.EventProcessing;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Polling;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Structure;
using PhilipDaubmeier.GraphIoT.Digitalstrom.ViewModel;
using PhilipDaubmeier.TokenStore.Database;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection
{
    public static class DigitalstromHostExtensions
    {
        public static IServiceCollection AddDigitalstromHost(this IServiceCollection serviceCollection, IConfiguration digitalstromConfig, IConfiguration tokenStoreConfig)
        {
            serviceCollection.Configure<DigitalstromConfig>(digitalstromConfig);

            serviceCollection.ConfigureTokenStore(tokenStoreConfig);
            serviceCollection.AddTokenStore<PersistingDigitalstromAuth>();

            serviceCollection.AddTransient<IDigitalstromConnectionProvider, DigitalstromConfigConnectionProvider>();
            serviceCollection.AddDssHttpClient();
            serviceCollection.AddScoped<DigitalstromDssClient>();

            serviceCollection.AddSingleton<IDigitalstromStructureService, DigitalstromStructureService>();

            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromEnergyPollingService>();
            serviceCollection.AddPollingService<IDigitalstromPollingService, DigitalstromSensorPollingService>();
            serviceCollection.AddTimedPollingHost<IDigitalstromPollingService>(digitalstromConfig.GetSection("PollingService"));
            serviceCollection.Configure<DigitalstromEventProcessingConfig>(digitalstromConfig.GetSection("EventProcessor"));

            serviceCollection.AddScoped<IDigitalstromEventProcessorPlugin, DssSceneEventProcessorPlugin>();
            serviceCollection.AddHostedService<DigitalstromEventsHostedService>();

            serviceCollection.AddGraphCollectionViewModel<DigitalstromEnergyViewModel>();
            serviceCollection.AddGraphCollectionViewModel<DigitalstromZoneSensorViewModel>();
            serviceCollection.AddEventCollectionViewModel<DigitalstromSceneEventViewModel>();

            return serviceCollection;
        }

        public static IServiceCollection AddDigitalstromHost<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbConfig, IConfiguration digitalstromConfig, IConfiguration tokenStoreConfig) where TDbContext : DbContext, IDigitalstromDbContext, ITokenStoreDbContext
        {
            serviceCollection.AddDbContext<IDigitalstromDbContext, TDbContext>(dbConfig);
            serviceCollection.AddTokenStoreDbContext<TDbContext>(dbConfig);

            return serviceCollection.AddDigitalstromHost(digitalstromConfig, tokenStoreConfig);
        }

        public static IServiceCollection AddDssHttpClient(this IServiceCollection serviceCollection)
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

            serviceCollection.AddHttpClient<DigitalstromConfigConnectionProvider>("DssHttpClient", client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(1); // Overall timeout across all tries
                })
                .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                {
                    return (serviceProvider.GetRequiredService<IDigitalstromConnectionProvider>() as DigitalstromConfigConnectionProvider)?.Handler;
                })
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutIndividualTryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            return serviceCollection;
        }
    }
}