using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.GraphIoT.App.Database;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Grafana.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Netatmo.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Sonnen.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Viessmann.DependencyInjection;
using System;

namespace PhilipDaubmeier.GraphIoT.App
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            void smarthomeSqlServer(DbContextOptionsBuilder options) =>
                options.UseSqlServer(Configuration.GetConnectionString("SmarthomeDB"));

            var networkConfig = Configuration.GetSection("Network");
            var tokenConfig = Configuration.GetSection("TokenStoreConfig");

            services.AddDbContext<PersistenceContext>(smarthomeSqlServer);

            services.AddLogging(config =>
            {
                config.ClearProviders();

                var logConfig = Configuration.GetSection("Logging");
                config.AddFile(logConfig);

                if (Environment.IsDevelopment())
                {
                    config.AddConfiguration(logConfig);
                    config.AddConsole();
                    config.AddDebug();
                    config.AddEventSourceLogger();
                }
            });

            services.AddLocalization(config => config.ResourcesPath = "Locale");

            services.AddOptions();

            services.AddSonnenHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("SonnenConfig"), tokenConfig, networkConfig);

            services.AddDigitalstromHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("DigitalstromConfig"), tokenConfig, networkConfig);

            services.AddNetatmoHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("NetatmoConfig"), tokenConfig, networkConfig);

            services.AddViessmannHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("ViessmannConfig"), tokenConfig, networkConfig);

            services.AddDatabaseBackupService<PersistenceContext>();

            services.AddGrafanaHost();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers())
               .ConfigureGrafanaHost("/graphiot");

            serviceProvider.GetRequiredService<PersistenceContext>().Database.Migrate();
        }
    }
}