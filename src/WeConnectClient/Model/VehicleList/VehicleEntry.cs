using PhilipDaubmeier.WeConnectClient.Model.Core;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    public class VehicleEntry
    {
        [JsonConverter(typeof(VinConverter))]
        public Vin Vin { get; set; } = Vin.Empty;

        public string Name { get; set; } = string.Empty;
        public bool Expired { get; set; }
        public string Model { get; set; } = string.Empty;
        public string ModelCode { get; set; } = string.Empty;
        public string ModelYear { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string VehicleSpecificFallbackImageUrl { get; set; } = string.Empty;
        public string ModelSpecificFallbackImageUrl { get; set; } = string.Empty;
        public string DefaultImageUrl { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string EnrollmentDate { get; set; } = string.Empty;
        public bool? DeviceOCU1 { get; set; }
        public bool? DeviceOCU2 { get; set; }
        public bool? DeviceMIB { get; set; }
        public bool EngineTypeCombustian { get; set; }
        public bool EngineTypeHybridOCU1 { get; set; }
        public bool EngineTypeHybridOCU2 { get; set; }
        public bool EngineTypeElectric { get; set; }
        public bool EngineTypeCNG { get; set; }
        public bool EngineTypeDefault { get; set; }
        public string StpStatus { get; set; } = string.Empty;
        public bool WindowstateSupported { get; set; }
        public string DashboardUrl { get; set; } = string.Empty;
        public bool VhrRequested { get; set; }
        public bool VsrRequested { get; set; }
        public bool VhrConfigAvailable { get; set; }
        public bool VerifiedByDealer { get; set; }
        public bool Vhr2 { get; set; }
        public bool RoleEnabled { get; set; }
        public bool IsEL2Vehicle { get; set; }
        public bool WorkshopMode { get; set; }
        public bool HiddenUserProfiles { get; set; }
        public bool? MobileKeyActivated { get; set; }
        public string EnrollmentType { get; set; } = string.Empty;
        public bool Ocu3Low { get; set; }
        public List<PackageService> PackageServices { get; set; }
        public bool DefaultCar { get; set; }
        public bool VwConnectPowerLayerAvailable { get; set; }
        public string XprofileId { get; set; } = string.Empty;
        public bool? SmartCardKeyActivated { get; set; }
        public bool FullyEnrolled { get; set; }
        public bool SecondaryUser { get; set; }
        public bool Fleet { get; set; }
        public bool Touareg { get; set; }
        public bool IceSupported { get; set; }
        public bool FlightMode { get; set; }
        public bool EsimCompatible { get; set; }
        public bool Dkyenabled { get; set; }
        public bool Selected { get; set; }
    }
}