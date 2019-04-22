using Microsoft.EntityFrameworkCore;

namespace PhilipDaubmeier.TokenStore.Database
{
    public class TokenStoreDbContext : DbContext
    {
        public DbSet<AuthData> AuthDataSet { get; set; }

        public TokenStoreDbContext(DbContextOptions<TokenStoreDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}