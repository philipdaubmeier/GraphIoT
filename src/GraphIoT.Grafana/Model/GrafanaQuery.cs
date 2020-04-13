using System.Collections.Generic;

namespace PhilipDaubmeier.GraphIoT.Grafana.Model
{
    public class GrafanaQuery
    {
        public string Timezone { get; set; } = string.Empty;
        public int? PanelId { get; set; }
        public int? DashboardId { get; set; }
        public Range Range { get; set; } = new Range();
        public RangeRaw RangeRaw { get; set; } = new RangeRaw();
        public string Interval { get; set; } = string.Empty;
        public long IntervalMs { get; set; }
        public List<TargetDefinition> Targets { get; set; } = new List<TargetDefinition>();
        public List<AdhocFilter>? AdhocFilters { get; set; } = new List<AdhocFilter>();
        public string Format { get; set; } = string.Empty;
        public int MaxDataPoints { get; set; }
    }

    public class TargetDefinition
    {
        public CustomQuery? Data { get; set; }
        public string Target { get; set; } = string.Empty;
        public string RefId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class CustomQuery
    {
        public string RawMetricId { get; set; } = string.Empty;
        public List<string>? FilterIfNoneOf { get; set; }
        public Aggregate? Aggregate { get; set; }
        public Correction? Correction { get; set; }
        public int? OverrideMaxDataPoints { get; set; } = null;
    }

    public class Aggregate
    {
        public string Interval { get; set; } = string.Empty;
        public string Func { get; set; } = string.Empty;
    }

    public class Correction
    {
        public double Factor { get; set; }
        public double Offset { get; set; }
    }

    public class AdhocFilter
    {
        public string Key { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}