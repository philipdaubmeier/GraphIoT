using System;

namespace SmarthomeApi.Clients.Netatmo
{
    public class NetatmoAuth
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }

        public bool MustAuthenticate()
        {
            bool expiredButCannotRefresh = Expiration.CompareTo(DateTime.UtcNow) < 0 && !MustRefreshToken();
            return !string.IsNullOrEmpty(AccessToken) || expiredButCannotRefresh;
        }

        public bool MustRefreshToken()
        {
            return !string.IsNullOrEmpty(RefreshToken) && Expiration.CompareTo(DateTime.UtcNow) < 0;
        }

        public string NetatmoAppId { get; set; }
        public string NetatmoAppSecret { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public string Scope { get { return "read_station read_presence access_presence"; } }
    }
}