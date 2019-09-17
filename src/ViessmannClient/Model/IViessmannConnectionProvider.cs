using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    public interface IViessmannConnectionProvider<T>
    {
        /// <summary>
        /// Credential information and a token store object.
        /// </summary>
        IViessmannAuth AuthData { get; }
        
        /// <summary>
        /// A HttpMessageHandler to inject for the connection to the Viessmann server,
        /// either for mocking purposes for testing or also for setting a proxy server etc.
        /// If null is returned, the Viessmann clients will create a default handler.
        /// </summary>
        HttpMessageHandler Handler { get; }

        string VitotrolDeviceId { get; }
        string VitotrolInstallationId { get; }

        string PlattformInstallationId { get; }
        string PlattformGatewayId { get; }
        string PlattformApiClientId { get; }
        string PlattformApiClientSecret { get; }
    }
}