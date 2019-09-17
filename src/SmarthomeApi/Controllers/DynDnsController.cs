using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TokenStore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Produces("application/json")]
    [Route("api/dyndns")]
    public class DynDnsController : Controller
    {
        private const int expiryDays = 7;
        private readonly PersistenceContext _db;
        private readonly TokenStore<DynDnsIpv4> _tokenStoreIPv4;
        private readonly TokenStore<DynDnsIpv6> _tokenStoreIPv6;

        public class DynDnsIpv4 { }
        public class DynDnsIpv6 { }

        public DynDnsController(TokenStore<DynDnsIpv4> tokenStoreIPv4, TokenStore<DynDnsIpv6> tokenStoreIPv6, PersistenceContext databaseContext)
        {
            _db = databaseContext;
            _tokenStoreIPv4 = tokenStoreIPv4;
            _tokenStoreIPv6 = tokenStoreIPv6;
        }

        // GET api/dyndns/update?username={username}&password={password}&domain={domain}&ipv4={ipv4}&ipv6={ipv6}
        [HttpGet("update")]
        public async Task<ActionResult> Update([FromQuery] string username, [FromQuery] string password, [FromQuery] string domain, [FromQuery] string ipv4, [FromQuery] string ipv6)
        {
            if (username != "philipdaubmeier" || password != "9bb04cf87b69e851")
                return StatusCode((int)HttpStatusCode.Forbidden);

            if (domain != "karlskron.daubmeier.de")
                return StatusCode((int)HttpStatusCode.NotFound);

            string ipv4ToWrite = null, ipv6ToWrite = null;
            if (!string.IsNullOrEmpty(ipv4))
            {
                if (!IPAddress.TryParse(ipv4, out IPAddress ip4Address))
                    return StatusCode((int)HttpStatusCode.BadRequest);

                if (ip4Address.AddressFamily != AddressFamily.InterNetwork)
                    return StatusCode((int)HttpStatusCode.BadRequest);

                if (!_tokenStoreIPv4.IsAccessTokenValid() || _tokenStoreIPv4.AccessToken != ip4Address.ToString())
                    ipv4ToWrite = ip4Address.ToString();
            }

            if (!string.IsNullOrEmpty(ipv6))
            {
                if (!IPAddress.TryParse(ipv6, out IPAddress ip6Address))
                    return StatusCode((int)HttpStatusCode.BadRequest);

                if (ip6Address.AddressFamily != AddressFamily.InterNetworkV6)
                    return StatusCode((int)HttpStatusCode.BadRequest);

                if (!_tokenStoreIPv6.IsAccessTokenValid() || _tokenStoreIPv6.AccessToken != ip6Address.ToString())
                    ipv6ToWrite = ip6Address.ToString();
            }

            if (ipv4ToWrite == null && ipv6ToWrite == null)
                return StatusCode((int)HttpStatusCode.NotModified);
            
            if (ipv4ToWrite != null)
                await _tokenStoreIPv4.UpdateToken(ipv4ToWrite, DateTime.Now.AddDays(expiryDays), string.Empty);
            
            if (ipv6ToWrite != null)
                await _tokenStoreIPv6.UpdateToken(ipv6ToWrite, DateTime.Now.AddDays(expiryDays), string.Empty);

            return StatusCode((int)HttpStatusCode.OK);
        }

        // GET api/dyndns/entries
        [HttpGet("entries")]
        public ActionResult GetEntries()
        {
            return Json(new
            {
                ipv4 = new
                {
                    address = _tokenStoreIPv4.AccessToken,
                    first_reported = _tokenStoreIPv4.AccessToken == null ? null : (DateTime?)_tokenStoreIPv4.AccessTokenExpiry.AddDays(-1 * expiryDays)
                },
                ipv6 = new
                {
                    address = _tokenStoreIPv6.AccessToken,
                    first_reported = _tokenStoreIPv6.AccessToken == null ? null : (DateTime?)_tokenStoreIPv6.AccessTokenExpiry.AddDays(-1 * expiryDays)
                }
            });
        }
    }
}