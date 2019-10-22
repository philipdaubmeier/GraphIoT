using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelZoneTest
    {
        [Theory]
        [InlineData(0, -1)]
        [InlineData(92, 92)]
        [InlineData(200, 200)]
        [InlineData(32007, 32007)]
        [InlineData(ushort.MaxValue, int.MaxValue)]
        [InlineData(0, int.MinValue)]
        public void TestBoundariesIntConversions(int expected, int inputVal)
        {
            Assert.Equal(expected, (int)(Zone)inputVal);
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(1, " 1")]
        [InlineData(1, "1 ")]
        [InlineData(1, "\t 1")]
        [InlineData(1, "1 \t\r\n ")]
        [InlineData(0, "-1")]
        [InlineData(104, "104")]
        [InlineData(ushort.MaxValue, "99999")]
        [InlineData(0, "foo")]
        public void TestBoundariesStringConversions(int expected, string inputVal)
        {
            Assert.Equal(expected, (int)(Zone)inputVal);
        }

        [Fact]
        public void TestEqualsOperators()
        {
            Zone zone1 = 7;
            Zone zone2 = 12;
            Zone zone3 = 12;

            Assert.False(zone1 == zone2);
            Assert.True(zone2 == zone3);

            Assert.True(zone1 != zone2);
            Assert.False(zone2 != zone3);

            Assert.False(zone1.Equals(zone2));
            Assert.True(zone2.Equals(zone3));
        }

        [Theory]
        [InlineData(68, 68)]
        [InlineData(19, 19)]
        public void TestGetHashCode(int expected, int inputVal)
        {
            Zone zone = inputVal;

            Assert.Equal(expected, zone.GetHashCode());
        }

        [Theory]
        [InlineData("Zone ID 7", 7)]
        [InlineData("Zone ID 68", 68)]
        [InlineData("Zone ID 32031", 32031)]
        public void TestToString(string expected, int inputVal)
        {
            Zone zone = inputVal;

            Assert.Equal(expected, zone.ToString());
        }
    }
}