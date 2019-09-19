using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromDssMock;
using PhilipDaubmeier.GraphIoT.Digitalstrom.Database;
using PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection;
using PhilipDaubmeier.DigitalstromTimeSeriesApi.Database;
using RichardSzalay.MockHttp;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class IntegrationTestWebHostFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public MockedRequest MockedEventResponse { get; private set; }

        public async Task<IntegrationTestDbContext> InitDb()
        {
            var dbContext = Server.Host.Services.GetRequiredService<IDigitalstromDbContext>() as IntegrationTestDbContext;

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            return dbContext;
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(null)
                          .UseStartup<TStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var integrationTestConfig = new ConfigurationBuilder()
                .AddJsonFile("integrationtestsettings.json")
                .Build();

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddConfiguration(integrationTestConfig);
            });

            builder.ConfigureServices(services =>
            {
                // Build a http mock for requests to the digitalstrom server
                services.AddSingleton(fac =>
                {
                    var mockHttp = new MockHttpMessageHandler();
                    mockHttp.AddAuthMock()
                        .AddEnergyMeteringMocks()
                        .AddSensorMocks()
                        .AddInitialAndSubscribeMocks();

                    MockedEventResponse = mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/event/get")
                            .WithExactQueryString($"subscriptionID=10&timeout=60000&token={MockDigitalstromConnection.AppToken}")
                            .Respond("application/json", SceneCommand.Preset0.ToMockedSceneEvent());

                    return mockHttp;
                });

                // Build a database context using an in-memory database for testing
                var dbRoot = new InMemoryDatabaseRoot();
                void dbConfig(DbContextOptionsBuilder options)
                {
                    options.UseInMemoryDatabase("InMemoryDbIntegrationTest", dbRoot);
                    options.UseInternalServiceProvider(new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider());
                }

                // Add all digitalstrom services using the mocked in-memory db
                services.AddDigitalstromHost<IntegrationTestDbContext>(dbConfig,
                    integrationTestConfig.GetSection("DigitalstromConfig"),
                    integrationTestConfig.GetSection("TokenStoreConfig")
                );

                // Replace the digitalstrom connection provider with a http mock for testing
                services.Remove(services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDigitalstromConnectionProvider)));
                services.AddTransient<IDigitalstromConnectionProvider, DigitalstromConnectionProvider>(fac =>
                    fac.GetRequiredService<MockHttpMessageHandler>().ToMockProvider());
            });
        }
    }
}