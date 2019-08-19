using System;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class BatterySystem
    {
        public string SerialNumber { get; set; }
        public string ProductName { get; set; }
        public string InstallerName { get; set; }
        public string InstallerStreet { get; set; }
        public string InstallerPostalCode { get; set; }
        public string InstallerCity { get; set; }
        public string InstallerState { get; set; }
        public string InstallerCountry { get; set; }
        public string InstallerPhone { get; set; }
        public string InstallerEmail { get; set; }
        public string InstallerAccountName { get; set; }
        public string InstallationDate { get; set; }
        public string InstallationStreet { get; set; }
        public string InstallationPostalCode { get; set; }
        public string InstallationCity { get; set; }
        public string InstallationState { get; set; }
        public string InstallationCountryCode { get; set; }
        public int? BatteryCapacity { get; set; }
        public int? BatteryModules { get; set; }
        public int? BatteryInverterDesignPower { get; set; }
        public string ControllerType { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public int? BatteryChargeCycles { get; set; }
        public double BackupPowerBuffer { get; set; }
        public string BackupDeviceType { get; set; }
        public int? BackupNominalPower { get; set; }
        public DateTime? LastPowerOutageAt { get; set; }
        public DateTime? LastMeasurementAt { get; set; }
        public string CellType { get; set; }
        public bool? Display { get; set; }
        public string ArticleNumber { get; set; }
        public string Color { get; set; }
        public string WarrantyPeriod { get; set; }
        public int? PvPeakPower { get; set; }
        public int? PvGridFeedInLimit { get; set; }
        public bool? HeaterConnectionStatus { get; set; }
        public double? HeaterMaxTemperature { get; set; }
        public bool? Online { get; set; }
    }
}