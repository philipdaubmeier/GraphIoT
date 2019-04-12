using System;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public class SystemEventName : IEventName
    {
        public enum EventType
        {
            ModelReady,
            Running,
            CallScene,
            CallSceneBus,
            UndoScene,
            Blink,
            ButtonClick,
            ButtonDeviceAction,
            DeviceBinaryInputEvent,
            DeviceSensorEvent,
            DeviceSensorValue,
            DeviceActionEvent,
            DeviceEventEvent,
            DeviceStateEvent,
            ZoneSensorValue,
            StateChange,
            Sunshine,
            FrostProtection,
            HeatingModeSwitch,
            BuildingService,
            Sendmail,
            OperationLock,
            ClusterConfigLock,
            DevicesFirstSeen,
            ActionExecute,
            Highlevelevent,
            ApartmentModelChanged,
            UserDefinedActionChanged,
            AddonStateChange,
            TimedEventChanged,
            ExecutionDenied,
            ExecutionDeniedDigestCheck,
            Unknown
        }

        public EventType type { get { return _type; } }

        private EventType _type;
        public string name
        {
            get
            {
                switch (_type)
                {
                    case EventType.ModelReady: return "model_ready";
                    case EventType.Running: return "running";
                    case EventType.CallScene: return "callScene";
                    case EventType.CallSceneBus: return "callSceneBus";
                    case EventType.UndoScene: return "undoScene";
                    case EventType.Blink: return "blink";
                    case EventType.ButtonClick: return "buttonClick";
                    case EventType.ButtonDeviceAction: return "buttonDeviceAction";
                    case EventType.DeviceBinaryInputEvent: return "deviceBinaryInputEvent";
                    case EventType.DeviceSensorEvent: return "deviceSensorEvent";
                    case EventType.DeviceSensorValue: return "deviceSensorValue";
                    case EventType.DeviceActionEvent: return "deviceActionEvent";
                    case EventType.DeviceEventEvent: return "deviceEventEvent";
                    case EventType.DeviceStateEvent: return "deviceStateEvent";
                    case EventType.ZoneSensorValue: return "zoneSensorValue";
                    case EventType.StateChange: return "stateChange";
                    case EventType.Sunshine: return "sunshine";
                    case EventType.FrostProtection: return "frostprotection";
                    case EventType.HeatingModeSwitch: return "heating_mode_switch";
                    case EventType.BuildingService: return "building_service";
                    case EventType.Sendmail: return "sendmail";
                    case EventType.OperationLock: return "operation_lock";
                    case EventType.ClusterConfigLock: return "cluster_config_lock";
                    case EventType.DevicesFirstSeen: return "devices_first_seen";
                    case EventType.ActionExecute: return "action_execute";
                    case EventType.Highlevelevent: return "highlevelevent";
                    case EventType.ApartmentModelChanged: return "apartmentModelChanged";
                    case EventType.UserDefinedActionChanged: return "userDefinedActionChanged";
                    case EventType.AddonStateChange: return "addonStateChange";
                    case EventType.TimedEventChanged: return "timedEventChanged";
                    case EventType.ExecutionDenied: return "executionDenied";
                    case EventType.ExecutionDeniedDigestCheck: return "execution_denied_digest_check";
                    default: return "unknown_event_type";
                }
            }
        }

        public SystemEventName(EventType type)
        {
            _type = type;
        }

        public static implicit operator SystemEventName(EventType type)
        {
            return new SystemEventName(type);
        }

        public static implicit operator SystemEventName(string name)
        {
            var matchingEvent = ((EventType[])Enum.GetValues(typeof(EventType)))
                .Select(x => new SystemEventName(x))
                .Where(x => x.name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (matchingEvent == null)
                return new SystemEventName(EventType.Unknown);
            return matchingEvent;
        }

        public static bool operator !=(SystemEventName name1, SystemEventName name2)
        {
            return !(name1 == name2);
        }

        public static bool operator ==(SystemEventName name1, SystemEventName name2)
        {
            if ((object)name1 == null || (object)name2 == null)
                return ReferenceEquals(name1, name2);
            return name1._type == name2._type;
        }

        public override bool Equals(object obj)
        {
            return ((SystemEventName)obj)._type == _type;
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }
    }
}