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

namespace PhilipDaubmeier.DigitalstromTimeSeriesApi
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
        
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionString = Configuration.GetConnectionString("DigitalstromTimeSeriesDB");
            return services.AddOptions()
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
                .AddGrafanaHost()
                .BuildServiceProvider();
        }
        
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc()
               .ConfigureGrafanaHost("/smarthome");

            var database = serviceProvider.GetRequiredService<DigitalstromTimeSeriesDbContext>().Database;
            if (!database.IsInMemory())
                database.Migrate();
        }
    }
}