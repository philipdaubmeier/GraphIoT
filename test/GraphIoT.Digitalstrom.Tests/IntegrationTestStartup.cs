using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Tests
{
    public class IntegrationTestStartup
    {
        public IntegrationTestStartup() { }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOptions();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}