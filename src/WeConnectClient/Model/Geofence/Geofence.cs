using PhilipDaubmeier.WeConnectClient.Model.Core;

namespace PhilipDaubmeier.WeConnectClient.Model.Geofence
{
    public class Geofence
    {
        public bool Active { get; set; }
        public string Id { get; set; } = string.Empty;
        public string DefinitionName { get; set; } = string.Empty;
        public string ZoneType { get; set; } = string.Empty;
        public string ShapeType { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int RotationAngle { get; set; }
        public int RectHeight { get; set; }
        public int RectWidth { get; set; }
        public int EllipseFirstRadius { get; set; }
        public int EllipseSecondRadius { get; set; }
        public bool Updated { get; set; }
        public Schedule Schedule { get; set; } = new Schedule();
        public string StartDateActive { get; set; } = string.Empty;
        public string TimeRangeActive { get; set; } = string.Empty;
    }
}