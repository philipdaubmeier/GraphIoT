using System;

namespace PhilipDaubmeier.WeConnectClient.Model.Auth
{
    internal class AuthState
    {
        public string Csrf { get; set; } = string.Empty;
        public string Referrer { get; set; } = string.Empty;
        public string BaseJsonUri { get; set; } = string.Empty;
        public string LoginUri { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string RelayStateToken { get; set; } = string.Empty;
        public string LoginFormUrl { get; set; } = string.Empty;
        public string HmacToken1 { get; set; } = string.Empty;
        public string HmacToken2 { get; set; } = string.Empty;
        public string LoginCsrf { get; set; } = string.Empty;
        public string PortletAuthCode { get; set; } = string.Empty;
        public string PortletAuthState { get; set; } = string.Empty;

        public Uri BaseUri => new Uri(new Uri(BaseJsonUri), "/");

        private bool forceRelogin = false;
        public void ForceRelogin() { forceRelogin = true; Reset(); }
        public bool MustForceRelogin() { var val = forceRelogin; forceRelogin = false; return val; }

        public void Reset() { Csrf = string.Empty; }
        public bool HasValidLogin() => !string.IsNullOrEmpty(BaseJsonUri) && !string.IsNullOrEmpty(Csrf);
    }
}