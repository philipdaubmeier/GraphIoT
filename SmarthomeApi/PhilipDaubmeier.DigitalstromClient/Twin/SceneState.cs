using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class SceneState : AbstractState<Scene>
    {
        public SceneState()
        {
            Value = SceneCommand.Unknown;
        }
    }
}