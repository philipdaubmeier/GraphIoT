using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CalendarHost.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Digitalstrom.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Grafana.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Netatmo.DependencyInjection;
using PhilipDaubmeier.GraphIoT.App.Clients.Withings;
using PhilipDaubmeier.GraphIoT.App.Controllers;
using PhilipDaubmeier.GraphIoT.App.Database;
using PhilipDaubmeier.GraphIoT.App.Model.Config;
using PhilipDaubmeier.GraphIoT.Sonnen.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Core.DependencyInjection;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Viessmann.DependencyInjection;
using System;

namespace PhilipDaubmeier.GraphIoT.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            Action<DbContextOptionsBuilder> smarthomeSqlServer = options =>
                options.UseSqlServer(Configuration.GetConnectionString("SmarthomeDB"));
            var tokenConfig = Configuration.GetSection("TokenStoreConfig");

            services.AddDbContext<PersistenceContext>(smarthomeSqlServer);

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConfiguration(Configuration.GetSection("Logging"));

                if (Environment.IsDevelopment())
                {
                    config.AddConsole();
                    config.AddDebug();
                    config.AddEventSourceLogger();
                }
            });

            services.AddOptions();
            services.Configure<AudiConnectConfig>(Configuration.GetSection("AudiConnectConfig"));
            services.Configure<WithingsConfig>(Configuration.GetSection("WithingsConfig"));
            services.Configure<TelegramConfig>(Configuration.GetSection("TelegramConfig"));

            services.ConfigureTokenStore(tokenConfig);
            services.AddTokenStoreDbContext<PersistenceContext>(smarthomeSqlServer);
            services.AddTokenStore<WithingsClient>();
            services.AddTokenStore<DynDnsController.DynDnsIpv4>();
            services.AddTokenStore<DynDnsController.DynDnsIpv6>();

            services.AddScoped<WithingsClient>();

            services.AddSonnenHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("SonnenConfig"), tokenConfig);

            services.AddDigitalstromHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("DigitalstromConfig"), tokenConfig);

            services.AddNetatmoHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("NetatmoConfig"), tokenConfig);

            services.AddViessmannHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("ViessmannConfig"), tokenConfig);

            services.AddCalendarHost<PersistenceContext>(smarthomeSqlServer);

            services.AddDatabaseBackupService<PersistenceContext>();

            services.AddGrafanaHost();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc()
               .ConfigureGrafanaHost("/smarthome");

            serviceProvider.GetRequiredService<PersistenceContext>().Database.Migrate();
        }
    }
}