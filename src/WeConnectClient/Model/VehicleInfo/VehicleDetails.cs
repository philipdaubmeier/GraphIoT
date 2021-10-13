using System.Collections.Generic;
using System.Diagnostics;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    public class VehicleDetails
    {
        public string Engine { get; set; } = string.Empty;
        public string ModelYear { get; set; } = string.Empty;
        public string ExteriorColorText { get; set; } = string.Empty;
        public List<Specification> Specifications { get; set; } = new();
    }

    [DebuggerDisplay("{CodeText}")]
    public class Specification
    {
        public string CodeText { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
    }
}