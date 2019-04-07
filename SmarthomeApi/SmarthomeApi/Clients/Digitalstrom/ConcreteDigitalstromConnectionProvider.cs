using DigitalstromClient.Model;
using DigitalstromClient.Network;
using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace SmarthomeApi.Clients.Digitalstrom
{
    public class ConcreteDigitalstromConnectionProvider : IDigitalstromConnectionProvider
    {
        private bool _useCloudredir = true;
        private bool _useProxy = true;

        private static IDigitalstromAuth digitalstromAuthData;

        private const string _dssTokenAppId = "SmarthomeAPI";
        private const string _dssUsername = "***REMOVED***";
        private const string _dssUserPassword = "***REMOVED***";

        private static Uri uriLocal = new Uri("***REMOVED***");
        private static Uri uriMyds = new Uri($"https://{_dsCloudDssId}.digitalstrom.net:8080/");
        private static Uri uriCloudredir = new Uri($"{_cloudredirBaseUrl}?adr={_dsCloudDssId}&token={_cloudredirToken}&path=");

        private const string _dsCloudDssId = "***REMOVED***";

        private const string _cloudredirBaseUrl = "https://ds-tools.net/cloudredir.php";
        private const string _cloudredirToken = "***REMOVED***";
        
        public UriPriorityList Uris
        {
            get
            {
                if (_useCloudredir)
                    return new UriPriorityList(new List<Uri>() { uriCloudredir }, new List<bool>() { true });
                else
                    return new UriPriorityList(new List<Uri>() { uriLocal, uriMyds });
            }
        }
        public IDigitalstromAuth AuthData { get; private set; }
        public WebProxy Proxy => _useProxy ? new WebProxy("127.0.0.1", 8033) : null;

        public ConcreteDigitalstromConnectionProvider(PersistenceContext db)
        {
            digitalstromAuthData = new PersistingDigitalstromAuth(db, _dssTokenAppId, _dssUsername, _dssUserPassword);
        }
    }
}