using DigitalstromClient.Model.Core;
using System.Collections.Generic;
using System.Linq;

namespace DigitalstromClient.Model.RoomState
{
    public class RoomState
    {
        private readonly Scene.SceneCommand _defaultScene = (new SceneState()).Value;
        private readonly Dictionary<Group, SceneState> _sceneStates;
        private readonly Dictionary<SensorType, SensorState> _sensorStates;

        public RoomState()
        {
            _sceneStates = Group.GetGroups().ToDictionary(x => x, _ => new SceneState());
            _sensorStates = SensorType.GetTypes().Where(x => x == SensorType.TypeValue.TemperatureIndoors
               || x == SensorType.TypeValue.HumidityIndoors
               || x == SensorType.TypeValue.RoomTemperatureSetpoint).ToDictionary(x => x, _ => new SensorState());
        }

        /// <summary>
        /// Returns true if the given color group has a scene value other than "Unknown"
        /// </summary>
        public bool HasSceneValue(Group color)
        {
            SceneState state;
            if (!_sceneStates.TryGetValue(color, out state))
                return false;
            return state.Value == _defaultScene;
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
                SceneState state;
                _sceneStates.TryGetValue(color, out state);
                if (state == null)
                {
                    state = new SceneState();
                    _sceneStates.Add(color, state);
                }
                return state;
            }
        }

        /// <summary>
        /// Returns true if the given sensor type has a valid value.
        /// </summary>
        public bool HasSensorValue(SensorType sensor)
        {
            SensorState state;
            if (!_sensorStates.TryGetValue(sensor, out state))
                return false;
            return state.Value.sensorType != SensorType.TypeValue.UnknownType;
        }

        /// <summary>
        /// Get a sensor value of the given sensor type in this room. The get accessor
        /// will always return a valid object and guarantees to never return null.
        /// Set a new value by getting it and writing into its Value property.
        /// </summary>
        /// <param name="sensor">The sensor type, e.g. temperature, humidity, etc.</param>
        /// <returns>The sensor state, e.g. 23°C, 54%, etc.</returns>
        public SensorState this[SensorType sensor]
        {
            get
            {
                SensorState state;
                _sensorStates.TryGetValue(sensor, out state);
                if (state == null)
                {
                    state = new SensorState();
                    _sensorStates.Add(sensor, state);
                }
                return state;
            }
        }
    }
}
