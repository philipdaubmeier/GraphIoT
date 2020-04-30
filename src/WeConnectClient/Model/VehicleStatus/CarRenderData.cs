namespace PhilipDaubmeier.WeConnectClient.Model.VehicleStatus
{
    public class CarRenderData
    {
        public int ParkingLights { get; set; }
        public int Hood { get; set; }
        public Doors Doors { get; set; } = new Doors();
        public Doors Windows { get; set; } = new Doors();
        public int Sunroof { get; set; }
    }
}