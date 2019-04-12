using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.RoomState
{
    public class SceneState : AbstractState<Scene>
    {
        public SceneState()
        {
            Value = Scene.SceneCommand.Unknown;
        }
    }
}