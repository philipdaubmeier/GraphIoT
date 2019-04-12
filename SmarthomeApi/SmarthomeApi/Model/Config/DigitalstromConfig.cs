using System;

namespace SmarthomeApi.Model.Config
{
    public class DigitalstromConfig
    {
        public string TokenAppId { get; set; }
        public string DssUsername { get; set; }
        public string DssPassword { get; set; }

        public string DssCertificate { get; set; }

        public string DssUriLocal { get; set; }
        public string DssUriDsNet { get; set; }
        public string DssUriCloudredir { get; set; }

        public string CloudDssId { get; set; }
        public string CloudredirToken { get; set; }

        public bool UseCloudredir { get; set; }

        public string Proxy { get; set; }
        public string ProxyPort { get; set; }
        
        public Uri UriLocal => string.IsNullOrWhiteSpace(DssUriLocal) ? null : new Uri(DssUriLocal);
        public Uri UriDsNet => new Uri(DssUriDsNet.Replace("{CloudDssId}", CloudDssId));
        public Uri UriCloudredir => new Uri(DssUriCloudredir.Replace("{CloudDssId}", CloudDssId).Replace("{CloudredirToken}", CloudredirToken));
    }
}