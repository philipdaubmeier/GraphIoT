using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class CommandList
    {
        public Command<CommandParamsSetName>? SetName { get; set; }
        public Command<CommandParamsSetCurve>? SetCurve { get; set; }
        public Command<CommandParamsSetSchedule>? SetSchedule { get; set; }
        public Command<CommandParamsSetMode>? SetMode { get; set; }
        public Command<CommandParamsSetTemperature>? SetTemperature { get; set; }
        public Command<CommandParamsNone>? Activate { get; set; }
        public Command<CommandParamsNone>? Deactivate { get; set; }
        public Command<CommandParamsChangeEndDate>? ChangeEndDate { get; set; }
        public Command<CommandParamsSchedule>? Schedule { get; set; }
        public Command<CommandParamsNone>? Unschedule { get; set; }
        public Command<CommandParamsSetTargetTemperature>? SetTargetTemperature { get; set; }

        public IEnumerable<ICommand> GetCommands()
        {
            if (SetName != null) yield return SetName;
            if (SetCurve != null) yield return SetCurve;
            if (SetSchedule != null) yield return SetSchedule;
            if (SetMode != null) yield return SetMode;
            if (SetTemperature != null) yield return SetTemperature;
            if (Activate != null) yield return Activate;
            if (Deactivate != null) yield return Deactivate;
            if (ChangeEndDate != null) yield return ChangeEndDate;
            if (Schedule != null) yield return Schedule;
            if (Unschedule != null) yield return Unschedule;
            if (SetTargetTemperature != null) yield return SetTargetTemperature;
        }

        public override string ToString() => $"{GetCommands().Count()} available commands";
    }

    public class CommandParamsNone { }

    public class CommandParamsSetName
    {
        public CommandParam<ConstraintString>? Name { get; set; }
    }

    public class CommandParamsSetCurve
    {
        public CommandParam<ConstraintNumber>? Slope { get; set; }
        public CommandParam<ConstraintNumber>? Shift { get; set; }
    }

    public class CommandParamsSetSchedule
    {
        public CommandParam<ConstraintSchedule>? NewSchedule { get; set; }
    }

    public class CommandParamsSetMode
    {
        public CommandParam<ConstraintEnum>? Mode { get; set; }
    }

    public class CommandParamsSetTemperature
    {
        public CommandParam<ConstraintNumber>? TargetTemperature { get; set; }
    }

    public class CommandParamsSetTargetTemperature
    {
        public CommandParam<ConstraintNumber>? Temperature { get; set; }
    }

    public class CommandParamsChangeEndDate
    {
        public CommandParam<ConstraintNone>? End { get; set; }
    }

    public class CommandParamsSchedule
    {
        public CommandParam<ConstraintNone>? Start { get; set; }
        public CommandParam<ConstraintNone>? End { get; set; }
    }

    public class CommandParam<TConstraint> where TConstraint : class
    {
        public bool? Required { get; set; }
        public string? Type { get; set; }
        public TConstraint? Constraints { get; set; }
    }

    public class ConstraintNone { }

    public class ConstraintString
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }

    public class ConstraintNumber
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public decimal? Stepping { get; set; }
    }

    public class ConstraintEnum
    {
        public List<string>? @Enum { get; set; }
    }

    public class ConstraintSchedule
    {
        public int? MaxEntries { get; set; }
        public int? Resolution { get; set; }
        public List<string>? Modes { get; set; }
        public string? DefaultMode { get; set; }
    }
}