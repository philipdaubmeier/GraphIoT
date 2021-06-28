using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public class NetatmoAuth : INetatmoAuth
    {
        public string? AccessToken { get; private set; }
        public DateTime AccessTokenExpiry { get; private set; }
        public string? RefreshToken { get; private set; }

        public string NetatmoAppId { get; set; } = string.Empty;
        public string NetatmoAppSecret { get; set; } = string.Empty;
        public string Username { get; private set; }
        public string UserPassword { get; private set; }
        public static string Scope { get { return "read_station read_presence access_presence"; } }

        public NetatmoAuth(string user, string password)
        {
            Username = user;
            UserPassword = password;

            AccessToken = null;
            AccessTokenExpiry = DateTime.MinValue;
            RefreshToken = null;
        }

        public bool IsAccessTokenValid() => !string.IsNullOrEmpty(AccessToken) && AccessTokenExpiry > DateTime.UtcNow;

        public bool MustAuthenticate()
        {
            bool expiredButCannotRefresh = AccessTokenExpiry.CompareTo(DateTime.UtcNow) < 0 && !MustRefreshToken();
            return !string.IsNullOrEmpty(AccessToken) || expiredButCannotRefresh;
        }

        public bool MustRefreshToken()
        {
            return !string.IsNullOrEmpty(RefreshToken) && AccessTokenExpiry.CompareTo(DateTime.UtcNow) < 0;
        }

        public Task UpdateTokenAsync(string? accessToken, DateTime accessTokenExpiry, string? refreshToken)
        {
            AccessToken = accessToken;
            AccessTokenExpiry = accessTokenExpiry;
            RefreshToken = refreshToken;
            return Task.CompletedTask;
        }
    }
}