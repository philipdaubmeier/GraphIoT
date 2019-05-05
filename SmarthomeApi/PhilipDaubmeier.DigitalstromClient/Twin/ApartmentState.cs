using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromClient.Twin
{
    public class ApartmentState : IEnumerable<KeyValuePair<Zone, RoomState>>, INotifyCollectionChanged
    {
        private readonly ConcurrentDictionary<Zone, RoomState> _dictionary = new ConcurrentDictionary<Zone, RoomState>();

        /// <summary>
        /// Event raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool IsRoomExisting(Zone zone)
        {
            return _dictionary.ContainsKey(zone);
        }

        public RoomState this[Zone zone]
        {
            get
            {
                _dictionary.TryGetValue(zone, out RoomState state);
                return state;
            }
            set
            {
                _dictionary[zone] = value;
                NotifyCollectionChangedAdded(zone, value);
            }
        }

        public SceneState this[Zone zone, Group group]
        {
            get
            {
                _dictionary.TryGetValue(zone, out RoomState state);
                if (state == null)
                    this[zone] = state = new RoomState();

                return state[group];
            }
        }

        public SensorState this[Zone zone, Sensor sensor]
        {
            get
            {
                _dictionary.TryGetValue(zone, out RoomState state);
                if (state == null)
                    this[zone] = state = new RoomState();

                return state[sensor];
            }
        }

        public IEnumerator<KeyValuePair<Zone, RoomState>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Notifies observers of CollectionChanged of an added item to the collection.
        /// </summary>
        private void NotifyCollectionChangedAdded(Zone zone, RoomState roomState)
        {
            var collectionHandler = CollectionChanged;
            if (collectionHandler == null)
                return;

            collectionHandler(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                new KeyValuePair<Zone, RoomState>(zone, roomState)));
        }
    }
}