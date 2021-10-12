using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public class WeConnectAuth : IWeConnectAuth
    {
        public string? AccessToken { get; private set; }
        public DateTime AccessTokenExpiry { get; private set; }

        public string Username { get; private set; }
        public string UserPassword { get; private set; }

        public WeConnectAuth(string user, string password)
        {
            Username = user;
            UserPassword = password;

            AccessToken = null;
            AccessTokenExpiry = DateTime.MinValue;
        }

        public bool IsAccessTokenValid() => !string.IsNullOrEmpty(AccessToken) && AccessTokenExpiry > DateTime.UtcNow;

        public Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken)
        {
            AccessToken = accessToken;
            AccessTokenExpiry = accessTokenExpiry;
            return Task.CompletedTask;
        }
    }
}