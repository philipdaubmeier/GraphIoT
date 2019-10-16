using Microsoft.Extensions.Options;
using PhilipDaubmeier.TokenStore.Database;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.TokenStore
{
    public class TokenStore<T>
    {
        private readonly string _serviceName;
        private readonly ITokenStoreDbContext _dbContext;
        private readonly Semaphore _dbSemaphore = new Semaphore(1, 1);

        public TokenStore(ITokenStoreDbContext databaseContext, IOptions<TokenStoreConfig> config)
        {
            string serviceName = string.Empty;
            if (!(config?.Value?.ClassNameMapping?.TryGetValue(typeof(T).Name, out serviceName) ?? false))
                serviceName = typeof(T).Name;

            _serviceName = serviceName;
            _dbContext = databaseContext;
        }

        private string AccessTokenId => $"{_serviceName}.access_token";
        private string AccessTokenExpiryId => $"{_serviceName}.access_token_expiry";
        private string RefreshTokenId => $"{_serviceName}.refresh_token";

        private string _accessToken = null;
        private DateTime? _accessTokenExpiry = null;
        private string _refreshToken = null;

        public string AccessToken => LoadTokenIfNull() ? _accessToken : null;
        public DateTime AccessTokenExpiry => LoadTokenIfNull() ? _accessTokenExpiry.Value : DateTime.MinValue;
        public string RefreshToken => LoadTokenIfNull() ? _refreshToken : null;

        private bool LoadTokenIfNull()
        {
            if (_accessToken != null && _accessTokenExpiry.HasValue)
                return true;

            try
            {
                _dbSemaphore.WaitOne();
                _accessToken = _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == AccessTokenId)?.DataContent;
                _accessTokenExpiry = FromBinaryString(_dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == AccessTokenExpiryId)?.DataContent);
                _refreshToken = _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == RefreshTokenId)?.DataContent;
                return true;
            }
            finally { _dbSemaphore.Release(); }
        }

        public async Task UpdateToken(string accessToken, DateTime accessTokenExpiry, string refreshToken)
        {
            try
            {
                _dbSemaphore.WaitOne();

                UpdateValue(AccessTokenId, _accessToken, accessToken);
                UpdateValue(AccessTokenExpiryId, _accessTokenExpiry.GetValueOrDefault().ToBinary().ToString(), accessTokenExpiry.ToBinary().ToString());
                UpdateValue(RefreshTokenId, _refreshToken, refreshToken);

                _accessToken = accessToken;
                _accessTokenExpiry = accessTokenExpiry;
                _refreshToken = refreshToken;

                await _dbContext.SaveChangesAsync();
            }
            finally { _dbSemaphore.Release(); }
        }

        public bool IsAccessTokenValid()
        {
            return AccessTokenExpiry > DateTime.Now && !string.IsNullOrEmpty(AccessToken);
        }

        private DateTime FromBinaryString(string str)
        {
            if (str == null)
                return DateTime.MinValue;

            if (!long.TryParse(str, out long time))
                return DateTime.MinValue;

            return DateTime.FromBinary(time);
        }

        private void UpdateValue(string key, string oldValue, string newValue)
        {
            if (oldValue == newValue)
                return;

            var entity = _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == key);
            if (entity == null)
                _dbContext.AuthDataSet.Add(new AuthData() { AuthDataId = key, DataContent = newValue });
            else
                entity.DataContent = newValue;
        }
    }
}