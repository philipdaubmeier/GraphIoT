using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class Wiremessage<T> : IWiremessage<T> where T : class
    {
        public DataWiremessage<T>? Data { get; set; }

        public T? ContainedData => Data?.Attributes;
    }

    public class ListWiremessage<T> : IWiremessage<List<T>> where T : class
    {
        public List<DataWiremessage<T>>? Data { get; set; }

        public List<T> ContainedData => Data?.Where(d => d.Attributes != null)?.Select(d => d.Attributes!)?.ToList() ?? new List<T>();
    }

    public class DataWiremessage<T> where T : class
    {
        [JsonConverter(typeof(IntToStringConverter))]
        public string? Id { get; set; }
        public string? Type { get; set; }
        public T? Attributes { get; set; }
        public LinkToSelf? Links { get; set; }
    }

    public class LinkToSelf
    {
        public string? Self { get; set; }
    }
}