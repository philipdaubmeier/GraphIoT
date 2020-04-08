using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PhilipDaubmeier.SonnenClient.Network
{
    public static class ConversionExtensions
    {
        public static string ToBase64UrlSafe(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static string ToBase64UrlSafe(this string str)
        {
            return Encoding.UTF8.GetBytes(str).ToBase64UrlSafe();
        }

        public static string ToSHA256Base64UrlSafe(this string str)
        {
            return new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(str)).ToBase64UrlSafe();
        }

        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            var querystring = uri.ToString().Substring(uri.ToString().IndexOf('?') + 1);
            var pairs = querystring.Split('&');
            var dict = pairs.Select(pair =>
            {
                var valuePair = pair.Split('=', 2);
                return new KeyValuePair<string, string>(valuePair[0], valuePair.Length <= 1 ? string.Empty : valuePair[1]);
            }).ToDictionary((kvp) => kvp.Key, (kvp) => kvp.Value);
            return dict;
        }

        public static string ReadHiddenHtmlInputValue(this string htmlPage, string name)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlPage);

            var node = htmlDoc.DocumentNode.SelectSingleNode($"//input[@type=\"hidden\"][@name=\"{name}\"]");

            return node.GetAttributeValue("value", null);
        }

        public static string ToFilterTime(this DateTime time)
        {
            return WebUtility.UrlEncode(time.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture));
        }
    }
}