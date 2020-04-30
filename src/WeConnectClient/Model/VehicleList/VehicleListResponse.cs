using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    internal class VehicleListResponse : IWiremessage<IEnumerable<VehicleEntry>>
    {
        public string ErrorCode { get; set; } = string.Empty;

        public bool HasError => !int.TryParse(ErrorCode, out int errorCode) || errorCode != 0;

        [JsonIgnore]
        public IEnumerable<VehicleEntry> Body
        {
            get
            {
                foreach (var entry in FullyLoadedVehiclesResponse.CompleteVehicles)
                    yield return entry;

                foreach (var entry in FullyLoadedVehiclesResponse.VehiclesNotFullyLoaded)
                    yield return entry;
            }
        }

        public VehicleListContainer FullyLoadedVehiclesResponse { get; set; } = new VehicleListContainer();
    }

    internal class VehicleListContainer
    {
        public List<VehicleEntry> CompleteVehicles { get; set; } = new List<VehicleEntry>();

        public List<VehicleEntry> VehiclesNotFullyLoaded { get; set; } = new List<VehicleEntry>();

        public string Status { get; set; } = string.Empty;

        public bool CurrentVehicleValid { get; set; }
    }
}