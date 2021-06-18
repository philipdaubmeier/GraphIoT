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

        string ClientId { get; }
        string CodeVerifier { get; }
        string CodeChallenge { get; }
        string CodeChallengeMethod { get; }
        string RedirectUri { get; }
    }
}