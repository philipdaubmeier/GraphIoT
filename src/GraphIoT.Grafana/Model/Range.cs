namespace PhilipDaubmeier.GraphIoT.Grafana.Model
{
    public class Range
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public RangeRaw? Raw { get; set; }
    }

    public class RangeRaw
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }
}