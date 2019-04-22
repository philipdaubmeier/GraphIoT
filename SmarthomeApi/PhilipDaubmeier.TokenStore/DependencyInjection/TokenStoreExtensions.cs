using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;

namespace PhilipDaubmeier.TokenStore.DependencyInjection
{
    public static class TokenStoreExtensions
    {
        public static IServiceCollection ConfigureTokenStore(this IServiceCollection serviceCollection, IConfiguration config = null)
        {
            var configSection = new TokenStoreConfig();
            config?.Bind(configSection);

            if (configSection.ConnectionString == null)
                return serviceCollection;

            return serviceCollection.AddDbContext<TokenStoreDbContext>(options =>
                                    {
                                        options.UseSqlServer(configSection.ConnectionString);
                                    })
                                    .Configure<TokenStoreConfig>(config);
        }

        public static IServiceCollection AddTokenStore<T>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton: return serviceCollection.AddSingleton<TokenStore<T>>();
                case ServiceLifetime.Transient: return serviceCollection.AddTransient<TokenStore<T>>();
                default: return serviceCollection.AddScoped<TokenStore<T>>();
            }
        }
    }
}