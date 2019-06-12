using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.TokenStore.Database
{
    public interface ITokenStoreDbContext : IDisposable
    {
        DbSet<AuthData> AuthDataSet { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}