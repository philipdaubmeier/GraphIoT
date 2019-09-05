using System;

namespace PhilipDaubmeier.NetatmoClient.Network
{
    public static class UriExtensions
    {
        public static Uri Combine(this Uri baseUri, string path)
        {
            string uri = baseUri.AbsoluteUri;
            if (uri.Length == 0 || path.Length == 0)
                return baseUri;

            string uri1 = uri.TrimEnd('/', '\\') + "/";
            string uri2 = path.TrimStart('/', '\\');

            return new Uri(new Uri(uri1, UriKind.Absolute), uri2);
        }
    }
}