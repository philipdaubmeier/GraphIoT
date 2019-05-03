using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Concurrent;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class ApartmentState : ObservableConcurrentDictionary<Zone, RoomState>
    {
        public bool IsRoomExisting(Zone zone)
        {
            return ContainsKey(zone);
        }

        public new RoomState this[Zone zone]
        {
            get
            {
                TryGetValue(zone, out RoomState state);
                return state;
            }
            set
            {
                base[zone] = value;
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