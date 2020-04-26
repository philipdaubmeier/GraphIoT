namespace PhilipDaubmeier.GraphIoT.Grafana.Model
{
    public class AnnotationQuery
    {
        public Range Range { get; set; } = new Range();
        public RangeRaw RangeRaw { get; set; } = new RangeRaw();
        public Annotation Annotation { get; set; } = new Annotation();
    }

    public class Annotation
    {
        public string Name { get; set; } = string.Empty;
        public string Datasource { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public bool Enable { get; set; }
        public string Query { get; set; } = string.Empty;
    }
}