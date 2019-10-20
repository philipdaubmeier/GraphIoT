using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.TokenStore.Database;
using System;

namespace PhilipDaubmeier.TokenStore.DependencyInjection
{
    public static class TokenStoreExtensions
    {
        public static IServiceCollection AddTokenStoreDbContext<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> dbOptionsAction) where TDbContext : DbContext, ITokenStoreDbContext
        {
            return serviceCollection.AddDbContext<ITokenStoreDbContext, TDbContext>(dbOptionsAction);
        }

        public static IServiceCollection ConfigureTokenStore(this IServiceCollection serviceCollection, IConfiguration config)
        {
            return serviceCollection.Configure<TokenStoreConfig>(config);
        }

        public static IServiceCollection AddTokenStore<T>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return serviceLifetime switch
            {
                ServiceLifetime.Singleton => serviceCollection.AddSingleton<TokenStore<T>>(),
                ServiceLifetime.Transient => serviceCollection.AddTransient<TokenStore<T>>(),
                _ => serviceCollection.AddScoped<TokenStore<T>>(),
            };
        }
    }
}