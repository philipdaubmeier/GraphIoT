using System;
using System.IO;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelDsuidTest
    {
        [Theory]
        [InlineData("0000000000000000000000000000000001", "1")]
        [InlineData("99999942f800000000000f0000deadbeef", "99999942f800000000000f0000deadbeef")]
        [InlineData("000000000000000000000000000000beef", "BEEF")]
        [InlineData("0000000000001234568790abcdefabcdef", "1234568790abcdefghijklmABCDEFGHIJKLM")]
        [InlineData("0000000000001234568790abcdefabcdef", "1234568790a  bcdefgh \t \r\n ijk  lmABCDEFGHIJKLM")]
        [InlineData("1234568790abcdef1234568790abcdef12", "1234568790abcdef1234568790abcdef1234568790abcdef1234568790abcdef")]
        public void TestConstructor(string expected, string input)
        {
            Dsuid dsuid = new(input);

            Assert.Equal(expected, dsuid.ToString());

            Dsuid dsuidAgain = new(dsuid.ToString());

            Assert.Equal(expected, dsuidAgain);
        }

        [Theory]
        [InlineData("0000000000000000000000000000000001")]
        [InlineData("99999942f800000000000f0000deadbeef")]
        [InlineData("000000000000000000000000000000beef")]
        [InlineData("0000000000001234568790abcdefabcdef")]
        [InlineData("1234568790abcdef1234568790abcdef12")]
        public void TestSerialization(string input)
        {
            Dsuid dsuidBeforeSerialization = new(input);
            Dsuid dsuidAfterSerialization;

            using (var stream = new MemoryStream())
            {
                dsuidBeforeSerialization.WriteTo(stream);

                stream.Seek(0, SeekOrigin.Begin);

                dsuidAfterSerialization = Dsuid.ReadFrom(stream);
            }

            Assert.Equal(dsuidBeforeSerialization, dsuidAfterSerialization);
        }

        [Fact]
        public void TestComparable()
        {
            Dsuid dsuid = new("0000000000001234568790abcdefabcdef");
            Dsuid dsuidToCompare = new("0000000000000000000000000000000001");
            IComparable dsuidComparable = dsuid;
            IComparable<Dsuid> dsuidGenericComparable = dsuid;

            Assert.True(dsuidComparable.CompareTo(dsuidToCompare) > 0);
            Assert.True(dsuidGenericComparable.CompareTo(dsuidToCompare) > 0);
        }

        [Fact]
        public void TestEquals()
        {
            Dsuid dsuid1 = new("0000000000001234568790abcdefabcdef");
            Dsuid dsuid2 = new("0000000000001234568790abcdefabcdef");
            Dsuid dsuid3 = new("0000000000000000000000000000000001");

            Assert.True(dsuid1.Equals(dsuid2));
            Assert.False(dsuid2.Equals(dsuid3));

            Assert.True(dsuid1 == dsuid2);
            Assert.False(dsuid2 == dsuid3);

            Assert.False(dsuid1 != dsuid2);
            Assert.True(dsuid2 != dsuid3);
        }

        [Fact]
        public void TestGetHashCode()
        {
            Dsuid dsuid11 = new("0000000000001234568790abcdefabcdef");
            Dsuid dsuid12 = new("0000000000001234568790abcdefabcdef");
            Dsuid dsuid21 = new("0000000000000000000000000000000001");
            Dsuid dsuid22 = new("0000000000000000000000000000000001");

            Assert.Equal(dsuid11.GetHashCode(), dsuid12.GetHashCode());
            Assert.Equal(dsuid21.GetHashCode(), dsuid22.GetHashCode());
        }

        [Fact]
        public void TestSize()
        {
            Assert.Equal(17, Dsuid.Size);
        }
    }
}