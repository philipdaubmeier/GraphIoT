using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class IntegrationTestStartup
    {
        public IntegrationTestStartup() { }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOptions();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}