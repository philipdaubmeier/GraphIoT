using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SmarthomeApi.Database.Model;
using SmarthomeApi.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmarthomeApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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

            services.AddHostedService<DigitalstromTimedHostedService>();
            services.AddHostedService<ViessmannTimedHostedService>();

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