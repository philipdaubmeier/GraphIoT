using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.DigitalstromHost.Database;
using System;

namespace PhilipDaubmeier.DigitalstromHost.Tests
{
    public class IntegrationTestWebHostFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<DigitalstromDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbIntegrationTest");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DigitalstromDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<IntegrationTestWebHostFactory<TStartup>>>();

                    try
                    {
                        // Ensure the database is created.
                        db.Database.EnsureCreated();

                        // TODO: Seed the database with test data here
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred creating and seeding the database. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}