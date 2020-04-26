using PhilipDaubmeier.NetatmoClient;
using PhilipDaubmeier.NetatmoClient.Network;
using PhilipDaubmeier.TokenStore;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Config
{
    public class NetatmoHostAuth : INetatmoAuth
    {
        private readonly TokenStore<NetatmoWebClient> _tokenStore;

        public string? AccessToken => _tokenStore.AccessToken;
        public DateTime AccessTokenExpiry => _tokenStore.AccessTokenExpiry;
        public string? RefreshToken => _tokenStore.RefreshToken;

        public string Username { get; private set; }
        public string UserPassword { get; private set; }

        public NetatmoHostAuth(TokenStore<NetatmoWebClient> tokenStore, string username, string password)
        {
            _tokenStore = tokenStore;
            Username = username;
            UserPassword = password;
        }

        public bool IsAccessTokenValid() => _tokenStore.IsAccessTokenValid();

        public bool MustAuthenticate()
        {
            bool expiredButCannotRefresh = AccessTokenExpiry.CompareTo(DateTime.UtcNow) < 0 && !MustRefreshToken();
            return !string.IsNullOrEmpty(AccessToken) || expiredButCannotRefresh;
        }

        public bool MustRefreshToken()
        {
            return !string.IsNullOrEmpty(RefreshToken) && AccessTokenExpiry.CompareTo(DateTime.UtcNow) < 0;
        }

        public async Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string? applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken ?? string.Empty, sessionExpiration, applicationToken ?? string.Empty);
        }
    }
}