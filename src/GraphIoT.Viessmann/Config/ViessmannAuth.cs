using PhilipDaubmeier.TokenStore;
using PhilipDaubmeier.ViessmannClient.Network;
using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Viessmann.Config
{
    public class ViessmannAuth<T> : IViessmannAuth
    {
        private readonly TokenStore<T> _tokenStore;

        public string? AccessToken => _tokenStore.AccessToken;
        public DateTime AccessTokenExpiry => _tokenStore.AccessTokenExpiry;
        public string? RefreshToken => _tokenStore.RefreshToken;

        public ViessmannAuth(TokenStore<T> tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public bool IsAccessTokenValid() => _tokenStore.IsAccessTokenValid();

        public async Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken)
        {
            await _tokenStore.UpdateToken(accessToken ?? string.Empty, accessTokenExpiry, refreshToken ?? string.Empty);
        }
    }
}