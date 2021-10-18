using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.WarningLights
{
    public class WarningLightsEntry
    {
        public DateTime CarCapturedTimestamp { get; set; } = DateTime.MinValue;

        [JsonPropertyName("mileage_km")]
        public double MileageKm { get; set; } = 0;

        public List<WarningEntry> WarningLights { get; set; } = new();
    }

    public class WarningEntry
    {
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public int NotificationId { get; set; } = 0;
        public bool ServiceLead { get; set; } = false;
        public bool CustomerRelevance { get; set; } = false;
        public DateTime TimeOfOccurrence { get; set; } = DateTime.MinValue;
        public string FieldActionCode { get; set; } = string.Empty;
        public string FieldActionCriteria { get; set; } = string.Empty;
    }
}