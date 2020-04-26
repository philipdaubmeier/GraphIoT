using System;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class BatterySystem
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string InstallerName { get; set; } = string.Empty;
        public string InstallerStreet { get; set; } = string.Empty;
        public string InstallerPostalCode { get; set; } = string.Empty;
        public string InstallerCity { get; set; } = string.Empty;
        public string InstallerState { get; set; } = string.Empty;
        public string InstallerCountry { get; set; } = string.Empty;
        public string InstallerPhone { get; set; } = string.Empty;
        public string InstallerEmail { get; set; } = string.Empty;
        public string InstallerAccountName { get; set; } = string.Empty;
        public string InstallationDate { get; set; } = string.Empty;
        public string InstallationStreet { get; set; } = string.Empty;
        public string InstallationPostalCode { get; set; } = string.Empty;
        public string InstallationCity { get; set; } = string.Empty;
        public string InstallationState { get; set; } = string.Empty;
        public string InstallationCountryCode { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public int? BatteryCapacity { get; set; }
        public int? BatteryModules { get; set; }
        public int? BatteryInverterDesignPower { get; set; }
        public string ControllerType { get; set; } = string.Empty;
        public string HardwareVersion { get; set; } = string.Empty;
        public string SoftwareVersion { get; set; } = string.Empty;
        public int? BatteryChargeCycles { get; set; }
        public double BackupPowerBuffer { get; set; }
        public string BackupDeviceType { get; set; } = string.Empty;
        public int? BackupNominalPower { get; set; }
        public DateTime? LastPowerOutageAt { get; set; }
        public DateTime? LastMeasurementAt { get; set; }
        public string CellType { get; set; } = string.Empty;
        public bool? Display { get; set; }
        public string ArticleNumber { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string WarrantyPeriod { get; set; } = string.Empty;
        public int? PvPeakPower { get; set; }
        public int? PvGridFeedInLimit { get; set; }
        public bool? HeaterConnectionStatus { get; set; }
        public double? HeaterMaxTemperature { get; set; }
        public bool? SmartSocketsEnabled { get; set; }
        public bool? Online { get; set; }
    }
}