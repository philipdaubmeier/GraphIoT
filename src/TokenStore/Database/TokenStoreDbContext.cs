using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.TokenStore.Database
{
    public class TokenStoreDbContext : DbContext, ITokenStoreDbContext
    {
        public DbSet<AuthData> AuthDataSet { get; set; }

        public TokenStoreDbContext(DbContextOptions<TokenStoreDbContext> options)
            : base(options)
        { }
    }
}