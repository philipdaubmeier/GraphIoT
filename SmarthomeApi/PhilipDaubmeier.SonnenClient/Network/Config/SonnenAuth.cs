using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenClient.Network
{
    public class SonnenAuth : ISonnenAuth
    {
        public string AccessToken { get; private set; }
        public DateTime AccessTokenExpiry { get; private set; }
        public string RefreshToken { get; private set; }

        public string Username { get; private set; }
        public string UserPassword { get; private set; }

        public SonnenAuth(string user, string password)
        {
            Username = user;
            UserPassword = password;

            AccessToken = null;
            AccessTokenExpiry = DateTime.MinValue;
            RefreshToken = null;
        }

        public bool IsAccessTokenValid() => !string.IsNullOrEmpty(AccessToken) && AccessTokenExpiry > DateTime.Now;

        public Task UpdateTokenAsync(string accessToken, DateTime accessTokenExpiry, string refreshToken)
        {
            AccessToken = accessToken;
            AccessTokenExpiry = accessTokenExpiry;
            RefreshToken = refreshToken;
            return Task.CompletedTask;
        }
    }
}