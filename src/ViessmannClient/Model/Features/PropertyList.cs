using PhilipDaubmeier.ViessmannClient.Model.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public enum Resolution
    {
        Day,
        Week,
        Month,
        Year
    }

    public class PropertyList
    {
        public TypedValue<object?>? Value { get; set; }
        public TypedValue<string?>? Status { get; set; }
        public TypedValue<bool>? Active { get; set; }
        public TypedValue<int>? ErrorCode { get; set; }
        public TypedValue<decimal>? Hours { get; set; }
        public TypedValue<long>? Starts { get; set; }
        public TypedValue<decimal>? OperationHours { get; set; }
        public TypedValue<decimal>? ProductionHours { get; set; }
        public TypedValue<List<string?>?>? Enabled { get; set; }
        public TypedValue<string?>? Name { get; set; }
        public TypedValue<decimal>? Shift { get; set; }
        public TypedValue<decimal>? Slope { get; set; }
        
        [JsonPropertyName("entries")]
        public TypedValue<ScheduleOrMessage>? ScheduleOrMessage { get; set; }

        public TypedValue<bool>? OverlapAllowed { get; set; }
        public TypedValue<decimal>? Temperature { get; set; }
        public TypedValue<string?>? Start { get; set; }
        public TypedValue<string?>? End { get; set; }
        public TypedValue<bool>? ServiceDue { get; set; }
        public TypedValue<int>? ServiceIntervalMonths { get; set; }
        public TypedValue<int>? ActiveMonthSinceLastService { get; set; }
        public TypedValue<string?>? LastService { get; set; }
        public TypedValue<List<double>?>? Day { get; set; }
        public TypedValue<List<double>?>? Week { get; set; }
        public TypedValue<List<double>?>? Month { get; set; }
        public TypedValue<List<double>?>? Year { get; set; }
        public TypedValue<string?>? Unit { get; set; }
        public TypedValue<bool>? Online { get; set; }
        public TypedValue<decimal>? Strength { get; set; }
        public TypedValue<int>? Channel { get; set; }
        public TypedValue<string>? UpdateStatus { get; set; }
        public TypedValue<string>? Version { get; set; }
        public TypedValue<List<Device>?>? Devices { get; set; }

        public int? ValueAsInt => (Value?.Value as IConvertible)?.ToInt32(null);

        public double? ValueAsDouble => (Value?.Value as IConvertible)?.ToDouble(null);

        public TypedValue<List<double>?>? SeriesByResolution(Resolution resolution) => resolution switch
        {
            Resolution.Day => Day,
            Resolution.Week => Week,
            Resolution.Month => Month,
            Resolution.Year => Year,
            _ => Day
        };

        [JsonIgnore]
        public TypedValue<Schedule>? Entries => ScheduleOrMessage?.Value.Schedule == null ? null :
            new TypedValue<Schedule>()
            {
                Type = ScheduleOrMessage?.Type,
                Value = ScheduleOrMessage?.Value.Schedule!
            };

        [JsonIgnore]
        public TypedValue<List<Message>?>? Messages => ScheduleOrMessage?.Value.Messages == null ? null : 
            new TypedValue<List<Message>?>()
            {
                Type = ScheduleOrMessage?.Type,
                Value = ScheduleOrMessage?.Value.Messages
            };

        public override string ToString()
        {
            return string.Join(", ", GetProperties().Select(p => $"{p.property}: {p.value}"));
        }

        public IEnumerable<(string property, Type type, object? value)> GetProperties()
        {
            if (Value != null) yield return ("value", typeof(object), Value.Value);
            if (Status != null) yield return ("status", typeof(string), Status.Value);
            if (Active != null) yield return ("active", typeof(bool), Active.Value);
            if (ErrorCode != null) yield return ("errorCode", typeof(int), ErrorCode.Value);
            if (Hours != null) yield return ("hours", typeof(decimal), Hours.Value);
            if (Starts != null) yield return ("starts", typeof(long), Starts.Value);
            if (Enabled != null) yield return ("enabled", typeof(List<string>), Enabled.Value);
            if (Name != null) yield return ("name", typeof(string), Name.Value);
            if (Shift != null) yield return ("shift", typeof(decimal), Shift.Value);
            if (Slope != null) yield return ("slope", typeof(decimal), Slope.Value);
            if (Entries != null) yield return ("entries", typeof(Schedule), Entries.Value);
            if (OverlapAllowed != null) yield return ("overlapAllowed", typeof(bool), OverlapAllowed.Value);
            if (Temperature != null) yield return ("temperature", typeof(decimal), Temperature.Value);
            if (Start != null) yield return ("start", typeof(string), Start.Value);
            if (End != null) yield return ("end", typeof(string), End.Value);
            if (ServiceDue != null) yield return ("serviceDue", typeof(bool), ServiceDue.Value);
            if (ServiceIntervalMonths != null) yield return ("serviceIntervalMonths", typeof(int), ServiceIntervalMonths.Value);
            if (ActiveMonthSinceLastService != null) yield return ("activeMonthSinceLastService", typeof(int), ActiveMonthSinceLastService.Value);
            if (LastService != null) yield return ("lastService", typeof(string), LastService.Value);
            if (Day != null) yield return ("day", typeof(List<double>), Day.Value);
            if (Week != null) yield return ("week", typeof(List<double>), Week.Value);
            if (Month != null) yield return ("month", typeof(List<double>), Month.Value);
            if (Year != null) yield return ("year", typeof(List<double>), Year.Value);
            if (Unit != null) yield return ("unit", typeof(string), Unit.Value);
        }
    }

    public class TypedValue<T>
    {
        public string? Type { get; set; }
        public T Value { get; set; } = default!;
    }
}