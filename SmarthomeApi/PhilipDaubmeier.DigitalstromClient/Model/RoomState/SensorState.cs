using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;

namespace PhilipDaubmeier.DigitalstromClient.Model.RoomState
{
    public class SensorState : AbstractState<SensorTypeAndValues>
    {
        public SensorState()
        {
            Value = new SensorTypeAndValues() { type = 0, time = 0, value = 0 };
        }
    }
}