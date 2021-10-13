namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    internal class VehicleDataResponse : VehicleData, IWiremessage<VehicleData>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        public virtual VehicleData Body => this;
    }
}