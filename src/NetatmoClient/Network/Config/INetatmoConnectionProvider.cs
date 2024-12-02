using System.Net.Http;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public interface INetatmoConnectionProvider
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        INetatmoAuth AuthData { get; }

        /// <summary>
        /// A HttpClient to inject for the connection to the netatmo server,
        /// to be used for working with HttpClientFactory pattern or for test mocking purposes.
        /// </summary>
        HttpClient Client { get; }

        string AppId { get; }

        string AppSecret { get; }

        string Scope { get; }

        string RedirectUri { get; }
    }
}