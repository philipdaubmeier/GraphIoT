﻿using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class SensorState : AbstractState<SensorTypeAndValues>
    {
        public SensorState()
        {
            Value = new SensorTypeAndValues()
            {
                Type = 0,
                Time = 0,
                Value = 0
            };
        }
    }
}