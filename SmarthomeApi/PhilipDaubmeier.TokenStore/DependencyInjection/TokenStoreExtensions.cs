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

        public static IServiceCollection ConfigureTokenStore(this IServiceCollection serviceCollection, IConfiguration config = null)
        {
            return serviceCollection.Configure<TokenStoreConfig>(config);
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