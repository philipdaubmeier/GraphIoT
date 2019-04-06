using System;

namespace DigitalstromClient.Model
{
    public class EphemeralDigitalstromAuth : IDigitalstromAuth, IDeepCloneable<EphemeralDigitalstromAuth>, IEquatable<EphemeralDigitalstromAuth>
    {
        public string ApplicationToken { get; set; }
        public string SessionToken { get; set; }
        public DateTime SessionExpiration { get; set; }
        
        public string AppId { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }

        public EphemeralDigitalstromAuth(string appId)
        {
            AppId = appId;
        }

        public bool MustFetchApplicationToken()
        {
            return string.IsNullOrEmpty(ApplicationToken);
        }

        public bool MustFetchSessionToken()
        {
            return string.IsNullOrEmpty(SessionToken) || SessionExpiration.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0;
        }

        public void TouchSessionToken()
        {
            SessionExpiration = DateTime.UtcNow.AddSeconds(60);
        }

        public EphemeralDigitalstromAuth DeepClone()
        {
            return new EphemeralDigitalstromAuth(AppId)
            {
                ApplicationToken = ApplicationToken,
                SessionToken = SessionToken,
                SessionExpiration = SessionExpiration,
                Username = Username,
                UserPassword = UserPassword
            };
        }

        public bool Equals(EphemeralDigitalstromAuth other)
        {
            return ApplicationToken == other.ApplicationToken && SessionToken == other.SessionToken
                && SessionExpiration == other.SessionExpiration && AppId == other.AppId
                && Username == other.Username && UserPassword == other.UserPassword;
        }
    }
}