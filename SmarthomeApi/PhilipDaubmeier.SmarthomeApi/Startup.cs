﻿using PhilipDaubmeier.DigitalstromClient.Model;
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
using ProxyKit;
using SmarthomeApi.Clients.Digitalstrom;
using SmarthomeApi.Database.Model;
using SmarthomeApi.Model.Config;
using SmarthomeApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmarthomeApi
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

            services.AddDbContext<PersistenceContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SmarthomeDB"));
            });

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

            services.AddOptions();
            services.Configure<AudiConnectConfig>(Configuration.GetSection("AudiConnectConfig"));
            services.Configure<DigitalstromConfig>(Configuration.GetSection("DigitalstromConfig"));
            services.Configure<NetatmoConfig>(Configuration.GetSection("NetatmoConfig"));
            services.Configure<SonnenConfig>(Configuration.GetSection("SonnenConfig"));
            services.Configure<ViessmannConfig>(Configuration.GetSection("ViessmannConfig"));
            services.Configure<WithingsConfig>(Configuration.GetSection("WithingsConfig"));

            services.AddTransient<IDigitalstromConnectionProvider, ConcreteDigitalstromConnectionProvider>();
            services.AddHostedService<DigitalstromEventsHostedService>();
            services.AddHostedService<DigitalstromTimedHostedService>();
            services.AddHostedService<ViessmannTimedHostedService>();
            services.AddHostedService<GrafanaBackendProcessService>();

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

            var dbService = serviceProvider.GetRequiredService<PersistenceContext>();
            dbService.Database.Migrate();
        }
    }
}