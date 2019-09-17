using System;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Events
{
    public enum SystemEvent
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

    public class SystemEventName : IEventName
    {
        public SystemEvent Type { get; }

        public string Name
        {
            get
            {
                switch (Type)
                {
                    case SystemEvent.ModelReady: return "model_ready";
                    case SystemEvent.Running: return "running";
                    case SystemEvent.CallScene: return "callScene";
                    case SystemEvent.CallSceneBus: return "callSceneBus";
                    case SystemEvent.UndoScene: return "undoScene";
                    case SystemEvent.Blink: return "blink";
                    case SystemEvent.ButtonClick: return "buttonClick";
                    case SystemEvent.ButtonDeviceAction: return "buttonDeviceAction";
                    case SystemEvent.DeviceBinaryInputEvent: return "deviceBinaryInputEvent";
                    case SystemEvent.DeviceSensorEvent: return "deviceSensorEvent";
                    case SystemEvent.DeviceSensorValue: return "deviceSensorValue";
                    case SystemEvent.DeviceActionEvent: return "deviceActionEvent";
                    case SystemEvent.DeviceEventEvent: return "deviceEventEvent";
                    case SystemEvent.DeviceStateEvent: return "deviceStateEvent";
                    case SystemEvent.ZoneSensorValue: return "zoneSensorValue";
                    case SystemEvent.StateChange: return "stateChange";
                    case SystemEvent.Sunshine: return "sunshine";
                    case SystemEvent.FrostProtection: return "frostprotection";
                    case SystemEvent.HeatingModeSwitch: return "heating_mode_switch";
                    case SystemEvent.BuildingService: return "building_service";
                    case SystemEvent.Sendmail: return "sendmail";
                    case SystemEvent.OperationLock: return "operation_lock";
                    case SystemEvent.ClusterConfigLock: return "cluster_config_lock";
                    case SystemEvent.DevicesFirstSeen: return "devices_first_seen";
                    case SystemEvent.ActionExecute: return "action_execute";
                    case SystemEvent.Highlevelevent: return "highlevelevent";
                    case SystemEvent.ApartmentModelChanged: return "apartmentModelChanged";
                    case SystemEvent.UserDefinedActionChanged: return "userDefinedActionChanged";
                    case SystemEvent.AddonStateChange: return "addonStateChange";
                    case SystemEvent.TimedEventChanged: return "timedEventChanged";
                    case SystemEvent.ExecutionDenied: return "executionDenied";
                    case SystemEvent.ExecutionDeniedDigestCheck: return "execution_denied_digest_check";
                    default: return "unknown_event_type";
                }
            }
        }

        public SystemEventName(SystemEvent type)
        {
            this.Type = type;
        }

        public static implicit operator SystemEventName(SystemEvent type)
        {
            return new SystemEventName(type);
        }

        public static implicit operator SystemEventName(string name)
        {
            var matchingEvent = ((SystemEvent[])Enum.GetValues(typeof(SystemEvent)))
                .Select(x => new SystemEventName(x))
                .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (matchingEvent == null)
                return new SystemEventName(SystemEvent.Unknown);
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
            return name1.Type == name2.Type;
        }

        public override bool Equals(object obj)
        {
            return ((SystemEventName)obj).Type == Type;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}