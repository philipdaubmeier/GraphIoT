using System;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelSceneTest
    {
        [Theory]
        [InlineData(0, SceneCommand.Preset0)]
        [InlineData(1, SceneCommand.Area1Off)]
        [InlineData(39, SceneCommand.Preset41)]
        [InlineData(92, SceneCommand.Unknown)]
        public void TestConstructorsAndImplicitConversions(int intVal, SceneCommand enumVal)
        {
            Scene sceneFromInt = intVal;
            Scene sceneFromStringImplicit = intVal.ToString();
            Scene sceneFromEnumImplicit = enumVal;
            Scene sceneFromEnumExplicit = new Scene(enumVal);

            // implicit conversions back to int
            Assert.Equal(intVal, (int)sceneFromInt);
            Assert.Equal((int)enumVal, (int)sceneFromStringImplicit);
            Assert.Equal((int)enumVal, (int)sceneFromEnumImplicit);
            Assert.Equal((int)enumVal, (int)sceneFromEnumExplicit);

            // implicit conversions back to enum and then to int
            Assert.Equal(intVal, (int)enumVal);
            Assert.Equal(intVal, (int)(SceneCommand)sceneFromInt);
            Assert.Equal((int)enumVal, (int)(SceneCommand)sceneFromStringImplicit);
            Assert.Equal((int)enumVal, (int)(SceneCommand)sceneFromEnumImplicit);
            Assert.Equal((int)enumVal, (int)(SceneCommand)sceneFromEnumExplicit);

            // all the same?
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromInt);
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromStringImplicit);
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromEnumImplicit);
        }

        [Theory]
        [InlineData(92, -1)]
        [InlineData(92, 92)]
        [InlineData(92, 200)]
        [InlineData(92, (int)SceneCommand.Reserved)]
        [InlineData(92, int.MaxValue)]
        [InlineData(92, int.MinValue)]
        public void TestBoundariesIntConversions(int expected, int inputVal)
        {
            Assert.Equal(expected, (int)(Scene)inputVal);
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(1, " 1")]
        [InlineData(1, "1 ")]
        [InlineData(1, "\t 1")]
        [InlineData(1, "1 \t\r\n ")]
        [InlineData(92, "-1")]
        [InlineData(92, "104")]
        [InlineData(92, "foo")]
        public void TestBoundariesStringConversions(int expected, string inputVal)
        {
            Assert.Equal(expected, (int)(Scene)inputVal);
        }

        [Fact]
        public void TestEqualsOperators()
        {
            Scene scene1 = new Scene(SceneCommand.DeepOff);
            Scene scene2 = new Scene(SceneCommand.Preset4);
            Scene scene3 = new Scene(SceneCommand.Preset4);

            Assert.False(scene1 == scene2);
            Assert.True(scene2 == scene3);

            Assert.True(scene1 != scene2);
            Assert.False(scene2 != scene3);

            Assert.False(scene1.Equals(scene2));
            Assert.True(scene2.Equals(scene3));
        }

        [Theory]
        [InlineData(68, SceneCommand.DeepOff)]
        [InlineData(19, SceneCommand.Preset4)]
        public void TestGetHashCode(int expected, SceneCommand inputVal)
        {
            Scene scene = new Scene(inputVal);

            Assert.Equal(HashCode.Combine(expected), scene.GetHashCode());
        }

        [Theory]
        [InlineData("DeepOff", SceneCommand.DeepOff)]
        [InlineData("Area1Stop", SceneCommand.Area1Stop)]
        [InlineData("Preset4", SceneCommand.Preset4)]
        public void TestToString(string expected, SceneCommand inputVal)
        {
            Scene scene = new Scene(inputVal);

            Assert.Equal(expected, scene.ToString());
        }
    }
}