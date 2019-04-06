using DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace DigitalstromClient.Model.RoomState
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
                RoomState state = null;
                _roomStates.TryGetValue(zone, out state);
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

        public SensorState this[Zone zone, SensorType sensor]
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
