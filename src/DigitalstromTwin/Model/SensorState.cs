using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class SensorState : AbstractState<SensorTypeAndValues>
    {
        public SensorState()
            : base(new SensorTypeAndValues()
            {
                Type = SensorType.UnknownType,
                Time = 0,
                Value = 0
            })
        { }
    }
}