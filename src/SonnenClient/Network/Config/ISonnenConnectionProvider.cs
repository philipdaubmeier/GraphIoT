using System.Net.Http;

namespace PhilipDaubmeier.SonnenClient.Network
{
    public interface ISonnenConnectionProvider
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        ISonnenAuth AuthData { get; }

        /// <summary>
        /// A HttpClient to inject for the connection to the my.sonnen.de server,
        /// to be used for working with HttpClientFactory pattern or for test mocking purposes.
        /// </summary>
        HttpClient Client { get; }

        string ClientId { get; }
    }
}