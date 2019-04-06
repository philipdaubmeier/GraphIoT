using DigitalstromClient.Model.Core;

namespace DigitalstromClient.Model.RoomState
{
    public class SceneState : AbstractState<Scene>
    {
        public SceneState()
        {
            Value = Scene.SceneCommand.Unknown;
        }
    }
}