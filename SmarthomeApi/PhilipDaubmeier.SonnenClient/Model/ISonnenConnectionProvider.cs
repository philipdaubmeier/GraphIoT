using System.Net.Http;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public interface ISonnenConnectionProvider
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        ISonnenAuth AuthData { get; }
        
        /// <summary>
        /// A HttpMessageHandler to inject for the connection to the my.sonnen.de server,
        /// either for mocking purposes for testing or also for setting a proxy server etc.
        /// If null is returned, SonnenClient will create a default handler.
        /// </summary>
        HttpMessageHandler Handler { get; }

        string ClientId { get; }
    }
}