using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelSensorTypeTest
    {
        [Theory]
        [InlineData(4, SensorType.ActivePower)]
        [InlineData(9, SensorType.TemperatureIndoors)]
        [InlineData(21, SensorType.CO2Concentration)]
        [InlineData(64, SensorType.OutputCurrent16A)]
        [InlineData(255, SensorType.UnknownType)]
        public void TestImplicitConversions(int intVal, SensorType enumVal)
        {
            Sensor sensorTypeFromInt = intVal;
            Sensor sensorTypeFromStringImplicit = intVal.ToString();
            Sensor sensorTypeFromEnumImplicit = enumVal;

            // implicit conversions back to int
            Assert.Equal(intVal, (int)sensorTypeFromInt);
            Assert.Equal((int)enumVal, (int)sensorTypeFromStringImplicit);
            Assert.Equal((int)enumVal, (int)sensorTypeFromEnumImplicit);

            // implicit conversions back to enum and then to int
            Assert.Equal(intVal, (int)enumVal);
            Assert.Equal(intVal, (int)(SensorType)sensorTypeFromInt);
            Assert.Equal((int)enumVal, (int)(SensorType)sensorTypeFromStringImplicit);
            Assert.Equal((int)enumVal, (int)(SensorType)sensorTypeFromEnumImplicit);

            // all the same?
            Assert.Equal((int)sensorTypeFromEnumImplicit, (int)sensorTypeFromInt);
            Assert.Equal((int)sensorTypeFromEnumImplicit, (int)sensorTypeFromStringImplicit);
        }

        [Theory]
        [InlineData(255, -1)]
        [InlineData(255, 0)]
        [InlineData(21, 21)]
        [InlineData(253, 253)]
        [InlineData(255, 32007)]
        [InlineData(255, int.MaxValue)]
        [InlineData(255, int.MinValue)]
        public void TestBoundariesIntConversions(int expected, int inputVal)
        {
            Assert.Equal(expected, (int)(Sensor)inputVal);
        }

        [Theory]
        [InlineData(4, "4")]
        [InlineData(4, " 4")]
        [InlineData(4, "4 ")]
        [InlineData(4, "\t 4")]
        [InlineData(4, "4 \t\r\n ")]
        [InlineData(255, "-1")]
        [InlineData(65, "65")]
        [InlineData(255, "99999")]
        [InlineData(255, "foo")]
        public void TestBoundariesStringConversions(int expected, string inputVal)
        {
            Assert.Equal(expected, (int)(Sensor)inputVal);
        }

        [Fact]
        public void TestEqualsOperators()
        {
            Sensor sensorType1 = null;
            Sensor sensorType2 = null;
            Sensor sensorType3 = 5;
            Sensor sensorType4 = 14;
            Sensor sensorType5 = 14;

            Assert.True(sensorType1 == sensorType2);
            Assert.False(sensorType2 == sensorType3);
            Assert.False(sensorType3 == sensorType4);
            Assert.True(sensorType4 == sensorType5);

            Assert.False(sensorType1 != sensorType2);
            Assert.True(sensorType2 != sensorType3);
            Assert.True(sensorType3 != sensorType4);
            Assert.False(sensorType4 != sensorType5);

            Assert.False(sensorType3.Equals(sensorType4));
            Assert.True(sensorType4.Equals(sensorType5));
        }

        [Theory]
        [InlineData(4, 4)]
        [InlineData(13, 13)]
        [InlineData(61, 61)]
        public void TestGetHashCode(int expected, int inputVal)
        {
            Sensor sensorType = inputVal;

            Assert.Equal(expected, sensorType.GetHashCode());
        }

        [Theory]
        [InlineData("SensorType 4: ActivePower", 4)]
        [InlineData("SensorType 13: HumidityIndoors", 13)]
        [InlineData("SensorType 21: CO2Concentration", 21)]
        [InlineData("SensorType 61: Reserved1", 61)]
        public void TestToString(string expected, int inputVal)
        {
            Sensor sensorType = inputVal;

            Assert.Equal(expected, sensorType.ToString());
        }

        [Fact]
        public void TestGetTypes()
        {
            var expected = new List<int>() { 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                20, 21, 22, 25, 50, 51, 60, 61, 62, 64, 65, 253, 255 };

            Assert.Equal(expected.Select(x => (Sensor)x), Sensor.GetTypes());

            foreach (var value in Enumerable.Range(-10, 300))
            {
                if (expected.Contains(value))
                    Assert.Equal(value, (int)(Sensor)value);
                else
                    Assert.Equal(255, (int)(Sensor)value);
            }
        }
    }
}