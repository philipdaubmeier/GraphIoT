using PhilipDaubmeier.WeConnectClient.Model.Core;
using System;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class AuthState
    {
        public string Csrf { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string RelayStateToken { get; set; } = string.Empty;
        public string HmacToken1 { get; set; } = string.Empty;
        public string HmacToken2 { get; set; } = string.Empty;
        public string LoginCsrf { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ReadableJwt AuthProxyVwAccessToken { get; set; } = string.Empty;
        public ReadableJwt AuthProxyWeConnectAccessToken { get; set; } = string.Empty;
        public ReadableJwt AccessToken { get; set; } = string.Empty;
    }
}