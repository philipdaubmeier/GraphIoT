using PhilipDaubmeier.WeConnectClient.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.Capabilities
{
    public class CapabilityList
    {
        [JsonConverter(typeof(VinConverter))]
        public Vin? Vin { get; set; } = null;
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string WorkshopMode { get; set; } = string.Empty;
        public string DevicePlatform { get; set; } = string.Empty;
        public List<Capability> Capabilities { get; set; } = new();
        public List<Article> Articles { get; set; } = new();
    }

    public class Capability
    {
        public string Id { get; set; } = string.Empty;
        public List<int> Status { get; set; } = new();
        public DateTime? ExpirationDate { get; set; } = null;
        public bool UserDisablingAllowed { get; set; } = false;
        public List<Action> Actions { get; set; } = new();
        public List<ServiceInfoList> Mbb { get; set; } = new();

        public IEnumerable<ServiceInfo> AllServiceInfos => Mbb.SelectMany(x => x.ServiceInfo);
    }

    public class ServiceInfoList
    {
        public List<ServiceInfo> ServiceInfo { get; set; } = new();
    }

    public class Action
    {
        public string Id { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public List<string> Scopes { get; set; } = new();
    }
}