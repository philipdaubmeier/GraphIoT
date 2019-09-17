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
        /// A HttpMessageHandler to inject for the connection to the netatmo server,
        /// either for mocking purposes for testing or also for setting a proxy server etc.
        /// If null is returned, NetatmoClient will create a default handler.
        /// </summary>
        HttpMessageHandler Handler { get; }

        string AppId { get; }

        string AppSecret { get; }

        string Scope { get; }
    }
}