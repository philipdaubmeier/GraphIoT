using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Geofence
{
    public class GeofenceCollection
    {
        public List<Geofence> GeoFenceList { get; set; } = new List<Geofence>();
        public int MaxNumberOfGeos { get; set; }
        public int MaxNumberOfActiveGeos { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TermAndConditionsURL { get; set; } = string.Empty;
    }
}