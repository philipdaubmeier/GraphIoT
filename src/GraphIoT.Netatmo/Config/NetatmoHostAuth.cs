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

        public NetatmoHostAuth(TokenStore<NetatmoWebClient> tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public bool IsAccessTokenValid() => _tokenStore.IsAccessTokenValid();

        public async Task UpdateTokenAsync(string? sessionToken, DateTime sessionExpiration, string? applicationToken)
        {
            await _tokenStore.UpdateToken(sessionToken ?? string.Empty, sessionExpiration, applicationToken ?? string.Empty);
        }
    }
}