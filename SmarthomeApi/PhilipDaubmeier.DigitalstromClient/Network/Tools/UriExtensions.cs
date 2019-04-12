using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    public class UriQueryStringBuilder
    {
        private Uri _uri;
        private NameValueCollection _values;

        public UriQueryStringBuilder(Uri uri, NameValueCollection values)
        {
            _values = values;
            _uri = uri;
        }

        public UriQueryStringBuilder AddQuery(string key, string value)
        {
            if (value != null)
                _values.Add(key, value);
            return this;
        }

        public UriQueryStringBuilder AddQuery(string key, int value)
        {
            _values.Add(key, value.ToString());
            return this;
        }

        public UriQueryStringBuilder AddQuery(string key, int? value)
        {
            return value.HasValue ? AddQuery(key, value.Value) : this;
        }

        public UriQueryStringBuilder AddQuery(string key, bool value)
        {
            _values.Add(key, value ? "true" : "false");
            return this;
        }

        public UriQueryStringBuilder AddQuery(string key, bool? value)
        {
            return value.HasValue ? AddQuery(key, value.Value) : this;
        }

        public UriQueryStringBuilder AddQuery(string key, List<KeyValuePair<string, string>> keyValueList)
        {
            if (keyValueList == null || keyValueList.Count == 0)
                return this;

            _values.Add(key, string.Join(";", keyValueList.Select(x => x.Key + "=" + x.Value).ToArray()));
            return this;
        }

        public static implicit operator UriQueryStringBuilder(Uri uri)
        {
            return new UriQueryStringBuilder(uri, new NameValueCollection());
        }

        public static implicit operator Uri(UriQueryStringBuilder builder)
        {
            var queryStr = new StringBuilder();

            // presumes that if there's a Query set, it starts with a ?
            var query = builder._uri.ToString().Split('?', 2).Skip(1).FirstOrDefault();
            var str = string.IsNullOrWhiteSpace(query) ? "?" : "&";

            foreach (var key in builder._values.AllKeys)
            {
                queryStr.Append(str + key + "=" + builder._values[key]);
                str = "&";
            }

            // query string will be encoded by building a new Uri instance
            // overwrites the existing query if it exists
            return new Uri(builder._uri + queryStr.ToString(), builder._uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }
    }

    public static class UriExtensions
    {
        public static UriQueryStringBuilder AddQuery(this Uri uri, string key, string value)
        {
            var values = new NameValueCollection();
            values.Add(key, value);
            return new UriQueryStringBuilder(uri, values);
        }

        public static UriQueryStringBuilder AddQuery(this Uri uri, string key, int value)
        {
            return uri.AddQuery(key, value.ToString());
        }

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