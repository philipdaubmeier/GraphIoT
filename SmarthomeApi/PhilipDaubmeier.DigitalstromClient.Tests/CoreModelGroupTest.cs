using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelGroupTest
    {
        [Theory]
        [InlineData(0, Group.Type.Various)]
        [InlineData(1, Group.Type.Light)]
        [InlineData(8, Group.Type.Joker)]
        [InlineData(64, Group.Type.ApartmentVentilation)]
        public void TestImplicitConversions(int intVal, Group.Type enumVal)
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
            Assert.Equal(intVal, (int)(Group.Type)groupFromInt);
            Assert.Equal((int)enumVal, (int)(Group.Type)groupFromStringImplicit);
            Assert.Equal((int)enumVal, (int)(Group.Type)groupFromEnumImplicit);

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
            Group group1 = null;
            Group group2 = null;
            Group group3 = Group.Type.Various;
            Group group4 = Group.Type.Light;
            Group group5 = Group.Type.Light;

            Assert.False(group1 == group3);
            Assert.False(group2 == group3);
            Assert.False(group3 == group4);
            Assert.True(group4 == group5);

            Assert.True(group1 != group3);
            Assert.True(group2 != group3);
            Assert.True(group3 != group4);
            Assert.False(group4 != group5);

            Assert.False(group3.Equals(group4));
            Assert.True(group4.Equals(group5));
        }

        [Theory]
        [InlineData(0, Group.Type.Various)]
        [InlineData(1, Group.Type.Light)]
        [InlineData(64, Group.Type.ApartmentVentilation)]
        public void TestGetHashCode(int expected, Group.Type inputVal)
        {
            Group group = inputVal;

            Assert.Equal(expected, group.GetHashCode());
        }

        [Theory]
        [InlineData("ID 0: White - Various", Group.Type.Various)]
        [InlineData("ID 1: Yellow - Light", Group.Type.Light)]
        [InlineData("ID 64: Blue - ApartmentVentilation", Group.Type.ApartmentVentilation)]
        public void TestToString(string expected, Group.Type inputVal)
        {
            Group group = inputVal;

            Assert.Equal(expected, group.ToString());
        }

        [Theory]
        [InlineData(Group.Color.White, Group.Type.Various)]
        [InlineData(Group.Color.Yellow, Group.Type.Light)]
        [InlineData(Group.Color.Gray, Group.Type.Shading)]
        [InlineData(Group.Color.Cyan, Group.Type.Audio)]
        [InlineData(Group.Color.Magenta, Group.Type.Video)]
        [InlineData(Group.Color.Red, Group.Type.Security)]
        [InlineData(Group.Color.Green, Group.Type.Access)]
        [InlineData(Group.Color.Black, Group.Type.Joker)]
        [InlineData(Group.Color.Blue, Group.Type.Heating)]
        [InlineData(Group.Color.Blue, Group.Type.Cooling)]
        [InlineData(Group.Color.Blue, Group.Type.Ventilation)]
        [InlineData(Group.Color.Blue, Group.Type.Window)]
        [InlineData(Group.Color.Blue, Group.Type.AirRecirculation)]
        [InlineData(Group.Color.Blue, Group.Type.ApartmentVentilation)]
        [InlineData(Group.Color.Blue, Group.Type.TemperatureControl)]
        public void TestGroupTypeToColor(Group.Color expected, Group.Type inputVal)
        {
            Group group = inputVal;

            Assert.Equal((int)expected, (int)(Group.Color)group);
        }

        [Fact]
        public void TestGetGroups()
        {
            var expected = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.Select(x => (Group)x);
            Assert.Equal(expected, Group.GetGroups());
        }
    }
}