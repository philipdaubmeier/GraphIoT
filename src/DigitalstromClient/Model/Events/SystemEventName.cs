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
                return Type switch
                {
                    SystemEvent.ModelReady => "model_ready",
                    SystemEvent.Running => "running",
                    SystemEvent.CallScene => "callScene",
                    SystemEvent.CallSceneBus => "callSceneBus",
                    SystemEvent.UndoScene => "undoScene",
                    SystemEvent.Blink => "blink",
                    SystemEvent.ButtonClick => "buttonClick",
                    SystemEvent.ButtonDeviceAction => "buttonDeviceAction",
                    SystemEvent.DeviceBinaryInputEvent => "deviceBinaryInputEvent",
                    SystemEvent.DeviceSensorEvent => "deviceSensorEvent",
                    SystemEvent.DeviceSensorValue => "deviceSensorValue",
                    SystemEvent.DeviceActionEvent => "deviceActionEvent",
                    SystemEvent.DeviceEventEvent => "deviceEventEvent",
                    SystemEvent.DeviceStateEvent => "deviceStateEvent",
                    SystemEvent.ZoneSensorValue => "zoneSensorValue",
                    SystemEvent.StateChange => "stateChange",
                    SystemEvent.Sunshine => "sunshine",
                    SystemEvent.FrostProtection => "frostprotection",
                    SystemEvent.HeatingModeSwitch => "heating_mode_switch",
                    SystemEvent.BuildingService => "building_service",
                    SystemEvent.Sendmail => "sendmail",
                    SystemEvent.OperationLock => "operation_lock",
                    SystemEvent.ClusterConfigLock => "cluster_config_lock",
                    SystemEvent.DevicesFirstSeen => "devices_first_seen",
                    SystemEvent.ActionExecute => "action_execute",
                    SystemEvent.Highlevelevent => "highlevelevent",
                    SystemEvent.ApartmentModelChanged => "apartmentModelChanged",
                    SystemEvent.UserDefinedActionChanged => "userDefinedActionChanged",
                    SystemEvent.AddonStateChange => "addonStateChange",
                    SystemEvent.TimedEventChanged => "timedEventChanged",
                    SystemEvent.ExecutionDenied => "executionDenied",
                    SystemEvent.ExecutionDeniedDigestCheck => "execution_denied_digest_check",
                    _ => "unknown_event_type",
                };
            }
        }

        public SystemEventName(SystemEvent type)
        {
            Type = type;
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
            if (name1 is null || name2 is null)
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