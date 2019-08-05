namespace PhilipDaubmeier.SonnenClient.Model
{
    public class MeasurementWiremessage<T> : IWiremessage<T> where T : class
    {
        public MeasurementAttributesWiremessage<T> Data { get; set; }

        public T ContainedData => Data?.Attributes;
    }

    public class MeasurementAttributesWiremessage<T> where T : class
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public T Attributes { get; set; }
    }
}