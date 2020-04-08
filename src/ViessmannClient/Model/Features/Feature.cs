using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    [DebuggerDisplay("Feature {Name,nq}")]
    public class Feature
    {
        [JsonPropertyName("feature")]
        public string? Name { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsReady { get; set; }
        public string? GatewayId { get; set; }
        public string? DeviceId { get; set; }
        public DateTime? Timestamp { get; set; }
        public PropertyList? Properties { get; set; }
        public CommandList? Commands { get; set; }
        public ComponentList? Components { get; set; }
    }
}