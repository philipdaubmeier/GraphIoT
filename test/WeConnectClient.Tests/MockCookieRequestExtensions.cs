using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockCookieRequestExtensions
    {
        public static MockedRequest WithCookies(this MockedRequest source, IEnumerable<KeyValuePair<string, string>> cookies)
        {
            return source.WithCookiesAndHeaders(new List<KeyValuePair<string, string>>(), cookies);
        }

        public static MockedRequest WithCookiesAndHeaders(this MockedRequest source, IEnumerable<KeyValuePair<string, string>> headers, IEnumerable<KeyValuePair<string, string>> cookies)
        {
            var cookieHeaderValue = string.Join("; ", cookies.Select(x => $"{x.Key}={x.Value}"));
            var header = new KeyValuePair<string, string>(MockCookieHttpMessageHandler.CookieHeaderKey, cookieHeaderValue);
            return source.WithHeaders(headers.Concat(new List<KeyValuePair<string, string>>() { header }));
        }
    }
}