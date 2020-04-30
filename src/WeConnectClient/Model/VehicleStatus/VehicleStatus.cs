namespace PhilipDaubmeier.WeConnectClient.Model.VehicleStatus
{
    public class VehicleStatus
    {
        public bool WindowStatusSupported { get; set; }
        public CarRenderData CarRenderData { get; set; } = new CarRenderData();
        public Doors LockData { get; set; } = new Doors();
        public bool LockDisabled { get; set; }
        public bool UnlockDisabled { get; set; }
        public bool RluDisabled { get; set; }
        public bool HideCngFuelLevel { get; set; }
        public int TotalRange { get; set; }
        public int? PrimaryEngineRange { get; set; }
        public int? FuelRange { get; set; }
        public int? CngRange { get; set; }
        public int? BatteryRange { get; set; }
        public int? FuelLevel { get; set; }
        public int? CngFuelLevel { get; set; }
        public int? BatteryLevel { get; set; }
        public string SliceRootPath { get; set; } = string.Empty;
    }
}