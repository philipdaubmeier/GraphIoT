using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class IntegrationTestStartup
    {
        public IntegrationTestStartup() { }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();

            return services.AddOptions()
                .BuildServiceProvider();
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}