using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Network
{
    public interface IViessmannConnectionProvider<T>
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        IViessmannAuth AuthData { get; }

        /// <summary>
        /// A HttpClient to inject for the connection to the Viessmann servers,
        /// to be used for working with HttpClientFactory pattern or for test mocking purposes.
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        /// A HttpClient to inject for authentication that does not follow redirects.
        /// </summary>
        HttpClient AuthClient { get; }

        string ClientId { get; }
        string RedirectUri { get; }
    }
}