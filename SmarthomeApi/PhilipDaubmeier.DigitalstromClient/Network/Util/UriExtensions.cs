using System;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    public static class UriExtensions
    {
        public static UriQueryStringBuilder AddQuery(this Uri uri, string key, string value)
        {
            var values = new NameValueCollection
            {
                { key, value }
            };
            return new UriQueryStringBuilder(uri, values);
        }

        public static UriQueryStringBuilder AddQuery(this Uri uri, string key, int value)
        {
            return uri.AddQuery(key, value.ToString());
        }
    }
}