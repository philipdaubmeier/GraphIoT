using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection;
using PhilipDaubmeier.DigitalstromTimeSeriesApi.Database;
using PhilipDaubmeier.GraphIoT.Grafana.DependencyInjection;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace PhilipDaubmeier.DigitalstromTimeSeriesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }
        
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var connectionString = Configuration.GetConnectionString("DigitalstromTimeSeriesDB");
            services.AddOptions()
                .AddLogging(config =>
                {
                    config.ClearProviders();
                    config.AddConfiguration(Configuration.GetSection("Logging"));

                    if (Environment.IsDevelopment())
                    {
                        config.AddConsole();
                        config.AddDebug();
                        config.AddEventSourceLogger();
                    }
                })
                .AddDbContext<DigitalstromTimeSeriesDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                })
                .AddDigitalstromHost<DigitalstromTimeSeriesDbContext>(options =>
                    {
                        options.UseSqlServer(connectionString);
                    },
                    Configuration.GetSection("DigitalstromConfig"),
                    Configuration.GetSection("TokenStoreConfig")
                )
                .AddGrafanaHost();
        }
        
        public virtual void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers())
               .ConfigureGrafanaHost("/graphiot");

            serviceProvider.GetRequiredService<DigitalstromTimeSeriesDbContext>().Database.Migrate();
        }
    }
}