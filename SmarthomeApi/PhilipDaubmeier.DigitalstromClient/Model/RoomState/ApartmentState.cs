using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.RoomState
{
    public class ApartmentState
    {
        private Dictionary<Zone, RoomState> _roomStates = new Dictionary<Zone, RoomState>();

        public bool IsRoomExisting(Zone zone)
        {
            return _roomStates.ContainsKey(zone);
        }
        
        public RoomState this[Zone zone]
        {
            get
            {
                _roomStates.TryGetValue(zone, out RoomState state);
                return state;
            }
            set
            {
                _roomStates[zone] = value;
            }
        }

        public SceneState this[Zone zone, Group group]
        {
            get
            {
                var room = this[zone];
                if (room == null)
                    return new SceneState();
                return room[group];
            }
        }

        public SensorState this[Zone zone, Sensor sensor]
        {
            get
            {
                var room = this[zone];
                if (room == null)
                    return new SensorState();
                return room[sensor];
            }
        }
    }
}