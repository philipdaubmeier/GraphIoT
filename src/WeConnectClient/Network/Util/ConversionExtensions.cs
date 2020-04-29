using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.WeConnectClient.Network
{
    public static class ConversionExtensions
    {
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

        public static void FormUrlEncoded(this HttpRequestMessage request, IEnumerable<(string, string)> values)
        {
            request.Content = new FormUrlEncodedContent(values.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)));
        }

        public static bool TryGetValue(this GroupCollection groupCollection, string key, out Group? value)
        {
            return ((IList<Group>)groupCollection).ToDictionary(g => g.Name, g => g).TryGetValue(key, out value);
        }

        public static bool TryExtractCsrf(this string body, out string csrf)
        {
            var csrfRegex = new Regex("<meta name=\"_csrf\" content=\"(?<csrf>.*?)\".*?/>");
            bool found = csrfRegex.Match(body).Groups.TryGetValue("csrf", out Group? group);
            csrf = found && group != null ? group.Value : string.Empty;
            return found;
        }

        public static bool TryExtractLoginHmac(this string body, out string hmac)
        {
            var csrfRegex = new Regex("<input.*?id=\"hmac\".*?value=\"(?<hmac>.*?)\".*?/>");
            bool found = csrfRegex.Match(body).Groups.TryGetValue("hmac", out Group? group);
            hmac = found && group != null ? group.Value : string.Empty;
            return found;
        }

        public static bool TryExtractLoginCsrf(this string body, out string csrf)
        {
            var csrfRegex = new Regex("<input.*?id=\"csrf\".*?value=\"(?<csrf>.*?)\".*?/>");
            bool found = csrfRegex.Match(body).Groups.TryGetValue("csrf", out Group? group);
            csrf = found && group != null ? group.Value : string.Empty;
            return found;
        }

        public static bool TryExtractUriParameter(this Uri uri, string paramName, out string value)
        {
            var found = uri.ParseQueryString().TryGetValue(paramName, out string? valueOrNull);
            value = valueOrNull ?? string.Empty;
            return found;
        }
    }
}