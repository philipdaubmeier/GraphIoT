using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.VehicleList
{
    public class PackageService
    {
        public string PackageServiceId { get; set; } = string.Empty;
        public string PropertyKeyReference { get; set; } = string.Empty;
        public string PackageServiceName { get; set; } = string.Empty;
        public string TrackingName { get; set; } = string.Empty;
        public string ActivationDate { get; set; } = string.Empty;
        public string ExpirationDate { get; set; } = string.Empty;
        public bool Expired { get; set; }
        public bool ExpireInAMonth { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public string EnrollmentPackageType { get; set; } = string.Empty;
    }
}