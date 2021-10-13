using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Capabilities
{
    public class Article
    {
        public string LpBundleNumber { get; set; } = string.Empty;
        public string SalesPartNumber { get; set; } = string.Empty;
        public DateTime ActivationDate { get; set; } = DateTime.MinValue;
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;
        public List<string> ServiceIds { get; set; } = new();
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool LifetimeLicense { get; set; } = false;
        public bool Available { get; set; } = false;
    }
}
