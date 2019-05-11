using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class UriQueryStringBuilder
    {
        private readonly Uri _uri;
        private readonly NameValueCollection _values;

        public UriQueryStringBuilder(Uri uri)
            : this(uri, new NameValueCollection())
        { }

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
            return new UriQueryStringBuilder(uri);
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
}