using PhilipDaubmeier.DigitalstromClient.Model;
using SmarthomeApi.Database.Model;
using System;
using System.Threading.Tasks;

namespace SmarthomeApi.Clients.Digitalstrom
{
    public class PersistingDigitalstromAuth : EphemeralDigitalstromAuth
    {
        private TokenStore _tokenStore;

        public new string ApplicationToken => _tokenStore.RefreshToken;
        public new string SessionToken => _tokenStore.AccessToken;
        public new DateTime SessionExpiration => _tokenStore.AccessTokenExpiry;

        public PersistingDigitalstromAuth(PersistenceContext db, string appId, string username, string password) : base(appId, username, password)
        {
            _tokenStore = new TokenStore(db, $"digitalstrom.{appId.ToLowerInvariant()}");
        }

        public new async Task UpdateTokenAsync(string sessionToken, DateTime sessionExpiration, string applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken, sessionExpiration, applicationToken);
        }
    }
}