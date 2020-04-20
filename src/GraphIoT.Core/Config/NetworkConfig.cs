using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.GraphIoT.Core.Config
{
    public class NetworkConfig
    {
        public ProxyConfig Proxy { get; set; } = new ProxyConfig();
    }

    public class ProxyConfig
    {
        public bool UseProxy { get; set; } = false;
        public string? Address { get; set; }
        public int? Port { get; set; } = null;

        public WebProxy? Proxy => !string.IsNullOrWhiteSpace(Address) && Port.HasValue ? new WebProxy(Address, Port.Value) : null;
    }

    public static class ProxyConfigExtensions
    {
        public static HttpClientHandler SetProxy(this HttpClientHandler handler, IOptions<NetworkConfig>? networkOptions)
        {
            // default: no proxy when no config is present
            if (networkOptions is null)
                return handler;

            var config = networkOptions.Value.Proxy;
            if (!config.UseProxy)
                return handler;

            if (config.Proxy is null)
                return handler;

            handler.UseProxy = config.UseProxy;
            handler.Proxy = config.Proxy;
            return handler;
        }
    }
}