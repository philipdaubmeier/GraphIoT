using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Capabilities
{
    public class CumulatedLicense
    {
        public string Status { get; set; } = string.Empty;
        public bool Warn { get; set; } = false;
        public string ActivationBy { get; set; } = string.Empty;
        public DateContent ActivationDate { get; set; } = new();
        public DateContent ExpirationDate { get; set; } = new();
        public LicenseProfiles LicenseProfiles { get; set; } = new();
    }

    public class LicenseProfiles
    {
        public string LastUsedProfile { get; set; } = string.Empty;
        public List<LicenseProfile> LicenseProfile { get; set; } = new();
    }

    public class DateContent
    {
        public DateTime Content { get; set; } = DateTime.MinValue;
    }
}
