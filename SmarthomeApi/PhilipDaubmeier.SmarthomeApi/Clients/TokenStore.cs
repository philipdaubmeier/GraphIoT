using PhilipDaubmeier.SmarthomeApi.Database.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Clients
{
    public class TokenStore
    {
        private string _serviceName;
        private TokenStoreDbContext _dbContext;

        public TokenStore(TokenStoreDbContext databaseContext, string serviceName)
        {
            _serviceName = serviceName;
            _dbContext = databaseContext;
        }
        
        private string _accessTokenId => $"{_serviceName}.access_token";
        private string _accessTokenExpiryId => $"{_serviceName}.access_token_expiry";
        private string _refreshTokenId => $"{_serviceName}.refresh_token";

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
            
            _accessToken = _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _accessTokenId)?.DataContent;
            _accessTokenExpiry = FromBinaryString(_dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _accessTokenExpiryId)?.DataContent);
            _refreshToken = _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _refreshTokenId)?.DataContent;
            return true;
        }

        public async Task UpdateToken(string accessToken, DateTime accessTokenExpiry, string refreshToken)
        {
            UpdateValue(_accessTokenId, _accessToken, accessToken);
            UpdateValue(_accessTokenExpiryId, _accessTokenExpiry.GetValueOrDefault().ToBinary().ToString(), accessTokenExpiry.ToBinary().ToString());
            UpdateValue(_refreshTokenId, _refreshToken, refreshToken);

            _accessToken = accessToken;
            _accessTokenExpiry = accessTokenExpiry;
            _refreshToken = refreshToken;

            await _dbContext.SaveChangesAsync();
        }

        public bool IsAccessTokenValid()
        {
            return AccessTokenExpiry > DateTime.Now && !string.IsNullOrEmpty(AccessToken);
        }

        private DateTime FromBinaryString(string str)
        {
            if (str == null)
                return DateTime.MinValue;

            long time;
            if (!long.TryParse(str, out time))
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