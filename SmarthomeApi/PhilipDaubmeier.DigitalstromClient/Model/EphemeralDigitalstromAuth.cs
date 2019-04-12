using System;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient.Model
{
    public class EphemeralDigitalstromAuth : IDigitalstromAuth, IEquatable<EphemeralDigitalstromAuth>
    {
        public string ApplicationToken { get; private set; }
        public string SessionToken { get; private set; }
        public DateTime SessionExpiration { get; private set; }
        
        public string AppId { get; private set; }
        public string Username { get; private set; }
        public string UserPassword { get; private set; }

        public EphemeralDigitalstromAuth(string appId, string username, string password)
        {
            AppId = appId;
            Username = username;
            UserPassword = password;
        }

        public bool MustFetchApplicationToken()
        {
            return string.IsNullOrEmpty(ApplicationToken);
        }

        public bool MustFetchSessionToken()
        {
            return string.IsNullOrEmpty(SessionToken) || SessionExpiration.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0;
        }

        public async Task TouchSessionTokenAsync()
        {
            await UpdateTokenAsync(SessionToken, DateTime.UtcNow.AddSeconds(60), ApplicationToken);
        }

        public Task UpdateTokenAsync(string sessionToken, DateTime sessionExpiration, string applicationToken)
        {
            ApplicationToken = applicationToken;
            SessionToken = sessionToken;
            SessionExpiration = sessionExpiration;
            return Task.CompletedTask;
        }

        public IDigitalstromAuth DeepClone()
        {
            return new EphemeralDigitalstromAuth(AppId, Username, UserPassword)
            {
                ApplicationToken = ApplicationToken,
                SessionToken = SessionToken,
                SessionExpiration = SessionExpiration
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