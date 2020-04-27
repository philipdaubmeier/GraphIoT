using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public interface IWeConnectConnectionProvider
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        IWeConnectAuth AuthData { get; }

        /// <summary>
        /// A HttpClient to inject for the connection to the WeConnect portal,
        /// to be used for working with HttpClientFactory pattern or for test mocking purposes.
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        /// A HttpClient to inject for authentication that does not follow redirects.
        /// </summary>
        HttpClient AuthClient { get; }

        /// <summary>
        /// The cookie container that will contain the session cookie needed for staying logged in.
        /// </summary>
        CookieContainer CookieContainer { get; }
    }
}