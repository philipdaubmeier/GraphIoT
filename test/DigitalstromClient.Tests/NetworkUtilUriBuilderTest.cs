using System;
using System.Collections.Generic;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Network.Tests
{
    public class NetworkUtilUriBuilderTest
    {
        [Theory]
        [InlineData("https://www.domain.tld/", UriKind.Absolute)]
        [InlineData("https://www.domain.tld/subpath", UriKind.Absolute)]
        [InlineData("/", UriKind.Relative)]
        [InlineData("/subpath/subsubpath/", UriKind.Relative)]
        public void TestImplicitConversions(string uriString, UriKind kind)
        {
            Uri uriBefore = new(uriString, kind);
            UriQueryStringBuilder builder = uriBefore;
            Uri uriAfter = builder;

            Assert.Equal(uriBefore, uriAfter);
            Assert.Equal(uriBefore.ToString(), uriAfter.ToString());
        }

        [Fact]
        public void TestChainedConversions()
        {
            Uri baseUri = new("/subpath/subsubpath/", UriKind.Relative);

            Uri newUri1 = baseUri.AddQuery("key1", "val1")
                .AddQuery("key2", "val2")
                .AddQuery("key3", "val3")
                .AddQuery("key4", "val4");

            Uri newUri2 =
                (
                    (Uri)
                        ((Uri)baseUri.AddQuery("key1", "val1").AddQuery("key2", "val2")
                    )
                    .AddQuery("key3", "val3")
                )
                .AddQuery("key4", "val4");

            Assert.Equal(newUri2, newUri1);
            Assert.Equal(newUri2.ToString(), newUri1.ToString());
        }

        [Fact]
        public void TestExtensionMethods()
        {
            Uri uri1 = new("https://www.domain.tld/", UriKind.Absolute);
            Uri uri2 = new("https://www.domain.tld/", UriKind.Absolute);
            Uri uri3 = new("/subpath/subsubpath/", UriKind.Relative);
            Uri uri4 = new("/subpath/subsubpath/", UriKind.Relative);

            int intvalue = 23;
            string strvalue = "foo";

            Uri uri1After = uri1.AddQuery("intvalue", intvalue);
            Uri uri2After = uri2.AddQuery("strvalue", strvalue);
            Uri uri3After = uri3.AddQuery("intvalue", intvalue);
            Uri uri4After = uri4.AddQuery("strvalue", strvalue);

            Assert.Equal("https://www.domain.tld/?intvalue=23", uri1After.ToString());
            Assert.Equal("https://www.domain.tld/?strvalue=foo", uri2After.ToString());
            Assert.Equal("/subpath/subsubpath/?intvalue=23", uri3After.ToString());
            Assert.Equal("/subpath/subsubpath/?strvalue=foo", uri4After.ToString());
        }

        [Fact]
        public void TestAddQueryInt()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            int intvalue = 23;

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("intvalue", intvalue);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("intvalue", intvalue);

            Assert.Equal("https://www.domain.tld/?intvalue=23", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?intvalue=23", ((Uri)uribuilder2After).ToString());
        }

        [Fact]
        public void TestAddQueryNullableInt()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);
            UriQueryStringBuilder uribuilder3 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder4 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            int? intvalue1 = 23;
            int? intvalue2 = null;

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("intvalue", intvalue1);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("intvalue", intvalue1);
            UriQueryStringBuilder uribuilder3After = uribuilder3.AddQuery("intvalue", intvalue2);
            UriQueryStringBuilder uribuilder4After = uribuilder4.AddQuery("intvalue", intvalue2);

            Assert.Equal("https://www.domain.tld/?intvalue=23", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?intvalue=23", ((Uri)uribuilder2After).ToString());
            Assert.Equal("https://www.domain.tld/", ((Uri)uribuilder3After).ToString());
            Assert.Equal("/subpath/subsubpath/", ((Uri)uribuilder4After).ToString());
        }

        [Fact]
        public void TestAddQueryBool()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            bool boolvalue = true;

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("boolvalue", boolvalue);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("boolvalue", boolvalue);

            Assert.Equal("https://www.domain.tld/?boolvalue=true", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?boolvalue=true", ((Uri)uribuilder2After).ToString());
        }

        [Fact]
        public void TestAddQueryNullableBool()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);
            UriQueryStringBuilder uribuilder3 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder4 = new Uri("/subpath/subsubpath/", UriKind.Relative);
            UriQueryStringBuilder uribuilder5 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder6 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            bool? boolvalue1 = true;
            bool? boolvalue2 = false;
            bool? boolvalue3 = null;

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("boolvalue", boolvalue1);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("boolvalue", boolvalue1);
            UriQueryStringBuilder uribuilder3After = uribuilder3.AddQuery("boolvalue", boolvalue2);
            UriQueryStringBuilder uribuilder4After = uribuilder4.AddQuery("boolvalue", boolvalue2);
            UriQueryStringBuilder uribuilder5After = uribuilder5.AddQuery("boolvalue", boolvalue3);
            UriQueryStringBuilder uribuilder6After = uribuilder6.AddQuery("boolvalue", boolvalue3);

            Assert.Equal("https://www.domain.tld/?boolvalue=true", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?boolvalue=true", ((Uri)uribuilder2After).ToString());
            Assert.Equal("https://www.domain.tld/?boolvalue=false", ((Uri)uribuilder3After).ToString());
            Assert.Equal("/subpath/subsubpath/?boolvalue=false", ((Uri)uribuilder4After).ToString());
            Assert.Equal("https://www.domain.tld/", ((Uri)uribuilder5After).ToString());
            Assert.Equal("/subpath/subsubpath/", ((Uri)uribuilder6After).ToString());
        }

        [Fact]
        public void TestAddQueryString()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            string strvalue = "foo";

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("strvalue", strvalue);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("strvalue", strvalue);

            Assert.Equal("https://www.domain.tld/?strvalue=foo", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?strvalue=foo", ((Uri)uribuilder2After).ToString());
        }

        [Fact]
        public void TestAddQueryKeysVals()
        {
            UriQueryStringBuilder uribuilder1 = new Uri("https://www.domain.tld/", UriKind.Absolute);
            UriQueryStringBuilder uribuilder2 = new Uri("/subpath/subsubpath/", UriKind.Relative);

            List<KeyValuePair<string, string>> keysvals = new()
            {
                new KeyValuePair<string, string>("key1", "val1"),
                new KeyValuePair<string, string>("key2", "val2")
            };

            UriQueryStringBuilder uribuilder1After = uribuilder1.AddQuery("keysvals", keysvals);
            UriQueryStringBuilder uribuilder2After = uribuilder2.AddQuery("keysvals", keysvals);

            Assert.Equal("https://www.domain.tld/?keysvals=key1=val1;key2=val2", ((Uri)uribuilder1After).ToString());
            Assert.Equal("/subpath/subsubpath/?keysvals=key1=val1;key2=val2", ((Uri)uribuilder2After).ToString());
        }

        [Theory]
        [InlineData("https://www.domain.tld/", UriKind.Absolute)]
        [InlineData("/subpath/subsubpath/", UriKind.Relative)]
        public void TestMultipleAddQuery(string uriString, UriKind kind)
        {
            Uri uri1 = new(uriString, kind);
            Uri uri2 = new(uriString, kind);
            UriQueryStringBuilder uribuilder1 = new Uri(uriString, kind);
            UriQueryStringBuilder uribuilder2 = new Uri(uriString, kind);

            Uri uriAfter1 = uri1.AddQuery("strval", "foo").AddQuery("intval", 23);
            Uri uriAfter2 = uri2.AddQuery("intval", 23).AddQuery("strval", "foo");
            Uri uriAfter3 = uribuilder1.AddQuery("strval", "foo").AddQuery("intval", 23);
            Uri uriAfter4 = uribuilder2.AddQuery("intval", 23).AddQuery("strval", "foo");

            Assert.Equal(uriString + "?strval=foo&intval=23", uriAfter1.ToString());
            Assert.Equal(uriString + "?intval=23&strval=foo", uriAfter2.ToString());
            Assert.Equal(uriString + "?strval=foo&intval=23", uriAfter3.ToString());
            Assert.Equal(uriString + "?intval=23&strval=foo", uriAfter4.ToString());
        }
    }
}