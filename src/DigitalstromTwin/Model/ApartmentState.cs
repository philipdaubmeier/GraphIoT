using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class ApartmentState : IEnumerable<KeyValuePair<Zone, RoomState>>, INotifyCollectionChanged
    {
        private readonly ConcurrentDictionary<Zone, RoomState> _dictionary = new();

        /// <summary>
        /// Event raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public bool IsRoomExisting(Zone zone)
        {
            return _dictionary.ContainsKey(zone);
        }

        public RoomState this[Zone zone]
        {
            get
            {
                bool added = false;
                var state = _dictionary.GetOrAdd(zone, _ => { added = true; return new RoomState(); });
                if (added)
                    NotifyCollectionChangedAdded(zone, state);

                return state;
            }
        }

        public SceneState this[Zone zone, Group group] => this[zone][group];

        public SensorState this[Zone zone, Sensor sensor] => this[zone][sensor];

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