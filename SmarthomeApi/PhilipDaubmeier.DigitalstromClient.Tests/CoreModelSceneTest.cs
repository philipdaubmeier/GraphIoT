using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core.Tests
{
    public class CoreModelSceneTest
    {
        [Theory]
        [InlineData(0, Scene.SceneCommand.Preset0)]
        [InlineData(1, Scene.SceneCommand.Area1Off)]
        [InlineData(39, Scene.SceneCommand.Preset41)]
        [InlineData(92, Scene.SceneCommand.Unknown)]
        public void TestConstructorsAndImplicitConversions(int intVal, Scene.SceneCommand enumVal)
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
            Assert.Equal(intVal, (int)(Scene.SceneCommand)sceneFromInt);
            Assert.Equal((int)enumVal, (int)(Scene.SceneCommand)sceneFromStringImplicit);
            Assert.Equal((int)enumVal, (int)(Scene.SceneCommand)sceneFromEnumImplicit);
            Assert.Equal((int)enumVal, (int)(Scene.SceneCommand)sceneFromEnumExplicit);

            // all the same?
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromInt);
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromStringImplicit);
            Assert.Equal((int)sceneFromEnumExplicit, (int)sceneFromEnumImplicit);
        }

        [Theory]
        [InlineData(92, -1)]
        [InlineData(92, 92)]
        [InlineData(92, 200)]
        [InlineData(92, (int)Scene.SceneCommand.Reserved)]
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
            Scene scene1 = null;
            Scene scene2 = null;
            Scene scene3 = new Scene(Scene.SceneCommand.DeepOff);
            Scene scene4 = new Scene(Scene.SceneCommand.Preset4);
            Scene scene5 = new Scene(Scene.SceneCommand.Preset4);

            Assert.False(scene1 == scene3);
            Assert.False(scene2 == scene3);
            Assert.False(scene3 == scene4);
            Assert.True(scene4 == scene5);

            Assert.True(scene1 != scene3);
            Assert.True(scene2 != scene3);
            Assert.True(scene3 != scene4);
            Assert.False(scene4 != scene5);

            Assert.False(scene3.Equals(scene4));
            Assert.True(scene4.Equals(scene5));
        }

        [Theory]
        [InlineData(68, Scene.SceneCommand.DeepOff)]
        [InlineData(19, Scene.SceneCommand.Preset4)]
        public void TestGetHashCode(int expected, Scene.SceneCommand inputVal)
        {
            Scene scene = new Scene(inputVal);

            Assert.Equal(expected, scene.GetHashCode());
        }

        [Theory]
        [InlineData("DeepOff", Scene.SceneCommand.DeepOff)]
        [InlineData("Area1Stop", Scene.SceneCommand.Area1Stop)]
        [InlineData("Preset4", Scene.SceneCommand.Preset4)]
        public void TestToString(string expected, Scene.SceneCommand inputVal)
        {
            Scene scene = new Scene(inputVal);

            Assert.Equal(expected, scene.ToString());
        }
    }
}