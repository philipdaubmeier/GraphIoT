using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class SceneState : AbstractState<Scene>
    {
        public SceneState() : base(SceneCommand.Unknown) { }
    }
}