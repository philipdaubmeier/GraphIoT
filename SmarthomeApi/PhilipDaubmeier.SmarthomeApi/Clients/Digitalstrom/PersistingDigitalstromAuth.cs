using PhilipDaubmeier.DigitalstromClient.Model;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Clients.Digitalstrom
{
    public class PersistingDigitalstromAuth : EphemeralDigitalstromAuth
    {
        private TokenStore<PersistingDigitalstromAuth> _tokenStore;

        public new string ApplicationToken => _tokenStore.RefreshToken;
        public new string SessionToken => _tokenStore.AccessToken;
        public new DateTime SessionExpiration => _tokenStore.AccessTokenExpiry;

        public PersistingDigitalstromAuth(TokenStore<PersistingDigitalstromAuth> tokenStore, string appId, string username, string password) : base(appId, username, password)
        {
            _tokenStore = tokenStore;
        }

        public new async Task UpdateTokenAsync(string sessionToken, DateTime sessionExpiration, string applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken, sessionExpiration, applicationToken);
        }
    }
}