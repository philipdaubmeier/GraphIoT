using PhilipDaubmeier.SonnenClient;
using PhilipDaubmeier.SonnenClient.Network;
using PhilipDaubmeier.TokenStore;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Config
{
    public class SonnenHostAuth : ISonnenAuth
    {
        private readonly TokenStore<SonnenPortalClient> _tokenStore;

        public string? AccessToken => _tokenStore.AccessToken;
        public DateTime AccessTokenExpiry => _tokenStore.AccessTokenExpiry;
        public string? RefreshToken => _tokenStore.RefreshToken;

        public string Username { get; }
        public string UserPassword { get; }

        public SonnenHostAuth(TokenStore<SonnenPortalClient> tokenStore, string username, string password)
        {
            _tokenStore = tokenStore;
            Username = username;
            UserPassword = password;
        }

        public bool IsAccessTokenValid() => _tokenStore.IsAccessTokenValid();

        public async Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string? applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken ?? string.Empty, sessionExpiration, applicationToken ?? string.Empty);
        }
    }
}