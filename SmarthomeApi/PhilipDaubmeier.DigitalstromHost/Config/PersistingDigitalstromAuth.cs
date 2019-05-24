using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.TokenStore;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromHost.Config
{
    public class PersistingDigitalstromAuth : EphemeralDigitalstromAuth
    {
        private readonly TokenStore<PersistingDigitalstromAuth> _tokenStore;

        public override string ApplicationToken => _tokenStore.RefreshToken;
        public override string SessionToken => _tokenStore.AccessToken;
        public override DateTime SessionExpiration => _tokenStore.AccessTokenExpiry;

        public PersistingDigitalstromAuth(TokenStore<PersistingDigitalstromAuth> tokenStore, string appId, string username, string password) : base(appId, username, password)
        {
            _tokenStore = tokenStore;
        }

        public override async Task UpdateTokenAsync(string sessionToken, DateTime sessionExpiration, string applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken, sessionExpiration, applicationToken);
        }
    }
}