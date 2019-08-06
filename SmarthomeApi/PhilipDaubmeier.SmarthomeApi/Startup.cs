using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CalendarHost.DependencyInjection;
using PhilipDaubmeier.DigitalstromHost.DependencyInjection;
using PhilipDaubmeier.SmarthomeApi.Clients.Withings;
using PhilipDaubmeier.SmarthomeApi.Controllers;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.SmarthomeApi.Model.Config;
using PhilipDaubmeier.SmarthomeApi.Services;
using PhilipDaubmeier.SonnenHost.DependencyInjection;
using PhilipDaubmeier.TokenStore.DependencyInjection;
using PhilipDaubmeier.ViessmannHost.DependencyInjection;
using ProxyKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.SmarthomeApi
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
            services
                .AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.RootDirectory = "/Views";
                    options.AllowMappingHeadRequestsToGetHandler = true;
                })
                .WithRazorPagesAtContentRoot();

            Action<DbContextOptionsBuilder> smarthomeSqlServer = options =>
                options.UseSqlServer(Configuration.GetConnectionString("SmarthomeDB"));
            
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

            services.AddProxy();
            services.AddHostedService<GrafanaBackendProcessService>();

            services.AddOptions();
            services.Configure<AudiConnectConfig>(Configuration.GetSection("AudiConnectConfig"));
            services.Configure<NetatmoConfig>(Configuration.GetSection("NetatmoConfig"));
            services.Configure<WithingsConfig>(Configuration.GetSection("WithingsConfig"));

            services.ConfigureTokenStore(Configuration.GetSection("TokenStoreConfig"));
            services.AddTokenStoreDbContext<PersistenceContext>(smarthomeSqlServer);
            services.AddTokenStore<WithingsClient>();
            services.AddTokenStore<DynDnsController.DynDnsIpv4>();
            services.AddTokenStore<DynDnsController.DynDnsIpv6>();

            services.AddScoped<WithingsClient>();

            services.AddSonnenHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("SonnenConfig"), Configuration.GetSection("TokenStoreConfig"));

            services.AddDigitalstromHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("DigitalstromConfig"), Configuration.GetSection("TokenStoreConfig"));

            services.AddViessmannHost<PersistenceContext>(smarthomeSqlServer, Configuration.GetSection("ViessmannConfig"), Configuration.GetSection("TokenStoreConfig"));

            services.AddCalendarHost<PersistenceContext>(smarthomeSqlServer);
            
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // reverse proxy for grafana
            var grafanaRegex = @"^(/smarthome)?/grafana($|/.*)";
            Func<string, string> rewriteFunc = (s) => new PathString(Regex.Matches(s, grafanaRegex).FirstOrDefault()?.Groups.Skip(2).FirstOrDefault()?.Value ?? string.Empty);
            app.UseWhen(context => Regex.IsMatch(context.Request.Path, grafanaRegex), appInner => appInner
                .UseRewriter(new RewriteOptions().Add(context => context.HttpContext.Request.Path = rewriteFunc(context.HttpContext.Request.Path)))
                .RunProxy(context => context.ForwardTo("http://localhost:8088").Send()));

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Views", "Style")),
                RequestPath = new PathString("/style"),
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { ".css", "text/css" },
                })
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Views", "Script")),
                RequestPath = new PathString("/script"),
                ContentTypeProvider = new FileExtensionContentTypeProvider(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { ".js", "application/javascript" },
                })
            });
            
            serviceProvider.GetRequiredService<PersistenceContext>().Database.Migrate();
        }
    }
}