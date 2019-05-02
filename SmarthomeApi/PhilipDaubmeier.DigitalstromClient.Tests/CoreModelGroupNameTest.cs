using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelGroupNameTest
    {
        [Theory]
        [InlineData("broadcast", GroupName.Name.Broadcast)]
        [InlineData("yellow", GroupName.Name.Yellow)]
        [InlineData("gray", GroupName.Name.Gray)]
        [InlineData("heating", GroupName.Name.Heating)]
        [InlineData("cooling", GroupName.Name.Cooling)]
        [InlineData("ventilation", GroupName.Name.Ventilation)]
        [InlineData("window", GroupName.Name.Window)]
        [InlineData("recirculation", GroupName.Name.Recirculation)]
        [InlineData("controltemperature", GroupName.Name.Controltemperature)]
        [InlineData("cyan", GroupName.Name.Cyan)]
        [InlineData("magenta", GroupName.Name.Magenta)]
        [InlineData("black", GroupName.Name.Black)]
        [InlineData("reserved1", GroupName.Name.Reserved1)]
        [InlineData("reserved2", GroupName.Name.Reserved2)]
        public void TestImplicitConversions(string stringVal, GroupName.Name enumVal)
        {
            GroupName groupNameFromString = stringVal;
            GroupName groupNameFromEnum = enumVal;

            // implicit conversions back to string
            Assert.Equal(stringVal, groupNameFromString);
            Assert.Equal(stringVal, groupNameFromEnum);
            Assert.Equal((string)groupNameFromString, groupNameFromEnum);

            // implicit conversions to enum and then to int
            Assert.Equal((int)(GroupName.Name)groupNameFromEnum, (int)(GroupName.Name)groupNameFromString);
        }
    }
}