using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmarthomeApi.Database.Model;

namespace SmarthomeApi.Database
{
    public static class AutoMigrator
    {
        public static void AutoMigrate(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<PersistenceContext>();

            context.Database.Migrate();
        }
    }
}