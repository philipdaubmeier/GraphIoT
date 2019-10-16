using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class RoomState : INotifyCollectionChanged
    {
        private readonly SceneCommand _defaultScene = (new SceneState()).Value;
        private readonly ConcurrentDictionary<Group, SceneState> _sceneStates = new ConcurrentDictionary<Group, SceneState>();
        private readonly ConcurrentDictionary<Sensor, SensorState> _sensorStates = new ConcurrentDictionary<Sensor, SensorState>();

        public IEnumerable<KeyValuePair<Group, SceneState>> Groups => _sceneStates;

        public IEnumerable<KeyValuePair<Sensor, SensorState>> Sensors => _sensorStates;

        /// <summary>
        /// Event raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Returns true if the given color group has a scene value other than "Unknown"
        /// </summary>
        public bool HasSceneValue(Group color)
        {
            if (!_sceneStates.TryGetValue(color, out SceneState state))
                return false;
            return state.Value != _defaultScene;
        }

        /// <summary>
        /// Get a scene state of the given color group in this room. The get accessor
        /// will always return a valid object and guarantees to never return null.
        /// Set a new value by getting it and writing into its Value property.
        /// </summary>
        /// <param name="color">The color group, e.g. yellow for light, grey for shadow, etc.</param>
        /// <returns>The scene, e.g. Off, Scene1, Scene2, etc.</returns>
        public SceneState this[Group color]
        {
            get
            {
                bool added = false;
                var state = _sceneStates.GetOrAdd(color, _ => { added = true; return new SceneState(); });
                if (added)
                    NotifyCollectionChangedAdded(color, state);

                return state;
            }
        }

        /// <summary>
        /// Returns true if the given sensor type has a valid value.
        /// </summary>
        public bool HasSensorValue(Sensor sensor)
        {
            if (!_sensorStates.TryGetValue(sensor, out SensorState state))
                return false;
            return state.Value.Type != SensorType.UnknownType;
        }

        /// <summary>
        /// Get a sensor value of the given sensor type in this room. The get accessor
        /// will always return a valid object and guarantees to never return null.
        /// Set a new value by getting it and writing into its Value property.
        /// </summary>
        /// <param name="sensor">The sensor type, e.g. temperature, humidity, etc.</param>
        /// <returns>The sensor state, e.g. 23°C, 54%, etc.</returns>
        public SensorState this[Sensor sensor]
        {
            get
            {
                bool added = false;
                var state = _sensorStates.GetOrAdd(sensor, _ => { added = true; return new SensorState(); });
                if (added)
                    NotifyCollectionChangedAdded(sensor, state);

                return state;
            }
        }

        /// <summary>
        /// Notifies observers of CollectionChanged of an added item to the collection.
        /// </summary>
        private void NotifyCollectionChangedAdded<TKey, TState>(TKey key, TState state)
        {
            var collectionHandler = CollectionChanged;
            if (collectionHandler == null)
                return;

            collectionHandler(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TState>(key, state)));
        }
    }
}