using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelGroupTest
    {
        [Theory]
        [InlineData(0, GroupType.Various)]
        [InlineData(1, GroupType.Light)]
        [InlineData(8, GroupType.Joker)]
        [InlineData(64, GroupType.ApartmentVentilation)]
        public void TestImplicitConversions(int intVal, GroupType enumVal)
        {
            Group groupFromInt = intVal;
            Group groupFromStringImplicit = intVal.ToString();
            Group groupFromEnumImplicit = enumVal;

            // implicit conversions back to int
            Assert.Equal(intVal, (int)groupFromInt);
            Assert.Equal((int)enumVal, (int)groupFromStringImplicit);
            Assert.Equal((int)enumVal, (int)groupFromEnumImplicit);

            // implicit conversions back to enum and then to int
            Assert.Equal(intVal, (int)enumVal);
            Assert.Equal(intVal, (int)(GroupType)groupFromInt);
            Assert.Equal((int)enumVal, (int)(GroupType)groupFromStringImplicit);
            Assert.Equal((int)enumVal, (int)(GroupType)groupFromEnumImplicit);

            // all the same?
            Assert.Equal((int)groupFromInt, (int)groupFromStringImplicit);
            Assert.Equal((int)groupFromInt, (int)groupFromEnumImplicit);
        }

        [Theory]
        [InlineData(0, -1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(8, 8)]
        [InlineData(12, 12)]
        [InlineData(0, 13)]
        [InlineData(0, 47)]
        [InlineData(48, 48)]
        [InlineData(0, 49)]
        [InlineData(0, 63)]
        [InlineData(64, 64)]
        [InlineData(64, 65)]
        [InlineData(64, int.MaxValue)]
        [InlineData(0, int.MinValue)]
        public void TestBoundariesIntConversions(int expected, int inputVal)
        {
            Assert.Equal(expected, (int)(Group)inputVal);
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(1, " 1")]
        [InlineData(1, "1 ")]
        [InlineData(1, "\t 1")]
        [InlineData(1, "1 \t\r\n ")]
        [InlineData(0, "-1")]
        [InlineData(64, "104")]
        [InlineData(0, "foo")]
        public void TestBoundariesStringConversions(int expected, string inputVal)
        {
            Assert.Equal(expected, (int)(Group)inputVal);
        }

        [Fact]
        public void TestEqualsOperators()
        {
            Group group1 = GroupType.Various;
            Group group2 = GroupType.Light;
            Group group3 = GroupType.Light;

            Assert.False(group1 == group2);
            Assert.True(group2 == group3);

            Assert.True(group1 != group2);
            Assert.False(group2 != group3);

            Assert.False(group1.Equals(group2));
            Assert.True(group2.Equals(group3));
        }

        [Theory]
        [InlineData(0, GroupType.Various)]
        [InlineData(1, GroupType.Light)]
        [InlineData(64, GroupType.ApartmentVentilation)]
        public void TestGetHashCode(int expected, GroupType inputVal)
        {
            Group group = inputVal;

            Assert.Equal(new Group(expected).GetHashCode(), group.GetHashCode());
        }

        [Theory]
        [InlineData("ID 0: White - Various", GroupType.Various)]
        [InlineData("ID 1: Yellow - Light", GroupType.Light)]
        [InlineData("ID 64: Blue - ApartmentVentilation", GroupType.ApartmentVentilation)]
        public void TestToString(string expected, GroupType inputVal)
        {
            Group group = inputVal;

            Assert.Equal(expected, group.ToString());
        }

        [Theory]
        [InlineData(Color.White, GroupType.Various)]
        [InlineData(Color.Yellow, GroupType.Light)]
        [InlineData(Color.Gray, GroupType.Shading)]
        [InlineData(Color.Cyan, GroupType.Audio)]
        [InlineData(Color.Magenta, GroupType.Video)]
        [InlineData(Color.Red, GroupType.Security)]
        [InlineData(Color.Green, GroupType.Access)]
        [InlineData(Color.Black, GroupType.Joker)]
        [InlineData(Color.Blue, GroupType.Heating)]
        [InlineData(Color.Blue, GroupType.Cooling)]
        [InlineData(Color.Blue, GroupType.Ventilation)]
        [InlineData(Color.Blue, GroupType.Window)]
        [InlineData(Color.Blue, GroupType.AirRecirculation)]
        [InlineData(Color.Blue, GroupType.ApartmentVentilation)]
        [InlineData(Color.Blue, GroupType.TemperatureControl)]
        public void TestGroupTypeToColor(Color expected, GroupType inputVal)
        {
            Group group = inputVal;

            Assert.Equal((int)expected, (int)(Color)group);
        }

        [Fact]
        public void TestGetGroups()
        {
            var expected = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.Select(x => (Group)x);
            Assert.Equal(expected, Group.GetGroups());
        }

        [Fact]
        public void TestGroupFormatting()
        {
            var group = (Group)2;
            Assert.Equal("Shade", group.ToString("d", CultureInfo.CreateSpecificCulture("en")));
            Assert.Equal("Shade", group.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
            Assert.Equal("Schatten", group.ToString("d", CultureInfo.CreateSpecificCulture("de-DE")));
            Assert.Equal("Schatten", group.ToString("d", CultureInfo.CreateSpecificCulture("de")));
        }
    }
}