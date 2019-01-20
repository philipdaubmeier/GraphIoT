using SmarthomeApi.Database.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients
{
    public class TokenStore
    {
        private string _serviceName;
        private PersistenceContext _dbContext;

        public TokenStore(PersistenceContext databaseContext, string serviceName)
        {
            _serviceName = serviceName;
            _dbContext = databaseContext;
        }
        
        private string _accessTokenId => $"{_serviceName}.access_token";
        private string _accessTokenExpiryId => $"{_serviceName}.access_token_expiry";
        private string _refreshTokenId => $"{_serviceName}.refresh_token";

        public string AccessToken => _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _accessTokenId)?.DataContent;
        public DateTime AccessTokenExpiry => FromBinaryString(_dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _accessTokenExpiryId)?.DataContent);
        public string RefreshToken => _dbContext.AuthDataSet.SingleOrDefault(x => x.AuthDataId == _refreshTokenId)?.DataContent;
        
        public async Task UpdateToken(string accessToken, DateTime accessTokenExpiry, string refreshToken)
        {
            UpdateValue(_accessTokenId, AccessToken, accessToken);
            UpdateValue(_accessTokenExpiryId, AccessTokenExpiry.ToBinary().ToString(), accessTokenExpiry.ToBinary().ToString());
            UpdateValue(_refreshTokenId, RefreshToken, refreshToken);
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
