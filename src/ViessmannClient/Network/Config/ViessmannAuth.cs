using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.ViessmannClient.Network
{
    public class ViessmannAuth : IViessmannAuth
    {
        public string? AccessToken { get; private set; }
        public DateTime AccessTokenExpiry { get; private set; }
        public string? RefreshToken { get; private set; }

        public ViessmannAuth()
        {
            AccessToken = null;
            AccessTokenExpiry = DateTime.MinValue;
            RefreshToken = null;
        }

        public bool IsAccessTokenValid() => !string.IsNullOrEmpty(AccessToken) && AccessTokenExpiry > DateTime.Now;

        public Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken)
        {
            AccessToken = accessToken;
            AccessTokenExpiry = accessTokenExpiry;
            RefreshToken = refreshToken;
            return Task.CompletedTask;
        }
    }
}