using DigitalstromClient.Model.Core;
using DigitalstromClient.Model.PropertyTree;

namespace DigitalstromClient.Model.RoomState
{
    public class SensorState : AbstractState<SensorTypeAndValues>
    {
        public SensorState()
        {
            Value = new SensorTypeAndValues() { type = 0, time = 0, value = 0 };
        }
    }
}