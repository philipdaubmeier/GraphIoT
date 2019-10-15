using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace PhilipDaubmeier.GraphIoT.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}